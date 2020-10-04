//  Beat Saber Custom Avatars - Custom player models for body presence in Beat Saber.
//  Copyright � 2018-2020  Beat Saber Custom Avatars Contributors
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomAvatar.Avatar;
using CustomAvatar.Player;
using CustomAvatar.Utilities;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CustomAvatar.UI
{
    internal class AvatarListViewController : BSMLResourceViewController, TableView.IDataSource
    {
        private const string kTableCellReuseIdentifier = "CustomAvatarsTableCell";

        public override string ResourceName => "CustomAvatar.Views.AvatarList.bsml";

        #pragma warning disable CS0649
        [UIComponent("avatar-list")] public CustomListTableData avatarList;
        [UIComponent("up-button")] public Button upButton;
        [UIComponent("down-button")] public Button downButton;
        #pragma warning restore CS0649
        
        private PlayerAvatarManager _avatarManager;

        private readonly List<AvatarListItem> _avatars = new List<AvatarListItem>();
        private LevelListTableCell _tableCellTemplate;

        private Texture2D _blankAvatarIcon;
        private Texture2D _noAvatarIcon;

        [Inject]
        private void Inject(PlayerAvatarManager avatarManager)
        {
            _avatarManager = avatarManager;
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            
            if (firstActivation)
            {
                _tableCellTemplate = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => x.name == "LevelListTableCell");
            
                _blankAvatarIcon = LoadTextureFromResource("CustomAvatar.Resources.mystery-man.png");
                _noAvatarIcon = LoadTextureFromResource("CustomAvatar.Resources.ban.png");

                avatarList.tableView.SetPrivateField("_pageUpButton", upButton);
                avatarList.tableView.SetPrivateField("_pageDownButton", downButton);
                avatarList.tableView.SetPrivateField("_hideScrollButtonsIfNotNeeded", false);
                
                TableViewScroller scroller = avatarList.tableView.GetPrivateField<TableViewScroller>("_scroller");

                upButton.onClick.AddListener(() =>
                {
                    scroller.PageScrollUp();
                    avatarList.tableView.RefreshScrollButtons(false);
                });

                downButton.onClick.AddListener(() =>
                {
                    scroller.PageScrollDown();
                    avatarList.tableView.RefreshScrollButtons(false);
                });
            
                avatarList.tableView.dataSource = this;
            }

            if (type == ActivationType.AddedToHierarchy)
            {
                _avatarManager.avatarChanged += OnAvatarChanged;

                _avatars.Clear();
                _avatars.Add(new AvatarListItem("No Avatar", _noAvatarIcon));
            
                _avatarManager.GetAvatarInfosAsync(avatar =>
                {
                    _avatars.Add(new AvatarListItem(avatar));

                    ReloadData();
                });
            }
        }

        private Texture2D LoadTextureFromResource(string resourceName)
        {
            Texture2D texture = new Texture2D(0, 0);

            using (Stream textureStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] textureBytes = new byte[textureStream.Length];
                textureStream.Read(textureBytes, 0, (int) textureStream.Length);
                texture.LoadImage(textureBytes);
            }

            return texture;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);

            if (deactivationType == DeactivationType.RemovedFromHierarchy)
            {
                _avatarManager.avatarChanged -= OnAvatarChanged;
            }
        }

        
        [UIAction("avatar-click")]
        private void OnAvatarClicked(TableView table, int row)
        {
            _avatarManager.SwitchToAvatarAsync(_avatars[row].fileName);
            avatarList.tableView.ScrollToCellWithIdx(row, TableViewScroller.ScrollPositionType.Center, true);
        }

        private void OnAvatarChanged(SpawnedAvatar avatar)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            _avatars.Sort((a, b) =>
            {
                if (string.IsNullOrEmpty(a.fileName)) return -1;
                if (string.IsNullOrEmpty(b.fileName)) return 1;

                return string.Compare(a.name, b.name, StringComparison.CurrentCulture);
            });

            int currentRow = _avatarManager.currentlySpawnedAvatar ? _avatars.FindIndex(a => a.fileName == _avatarManager.currentlySpawnedAvatar.avatar.fileName) : 0;
            
            avatarList.tableView.ReloadData();
            avatarList.tableView.ScrollToCellWithIdx(currentRow, TableViewScroller.ScrollPositionType.Center, true);
            avatarList.tableView.SelectCellWithIdx(currentRow);
        }

        public float CellSize()
        {
            return 8.5f;
        }

        public int NumberOfCells()
        {
            return _avatars.Count;
        }

        public TableCell CellForIdx(TableView tableView, int idx)
        {
            LevelListTableCell tableCell = avatarList.tableView.DequeueReusableCellForIdentifier(kTableCellReuseIdentifier) as LevelListTableCell;

            if (!tableCell)
            {
                tableCell = Instantiate(_tableCellTemplate);

                foreach (var image in tableCell.GetPrivateField<Image[]>("_beatmapCharacteristicImages"))
                {
                    DestroyImmediate(image);
                }

                tableCell.SetPrivateField("_beatmapCharacteristicImages", new UnityEngine.UI.Image[0]);
                tableCell.GetPrivateField<RawImage>("_favoritesBadgeImage").enabled = false;

                tableCell.reuseIdentifier = kTableCellReuseIdentifier;
            }

            AvatarListItem avatar = _avatars[idx];

            tableCell.GetPrivateField<TextMeshProUGUI>("_songNameText").text = avatar.name;
            tableCell.GetPrivateField<TextMeshProUGUI>("_authorText").text = avatar.author;
            tableCell.GetPrivateField<RawImage>("_coverRawImage").texture = avatar.icon ? avatar.icon : _blankAvatarIcon;

            return tableCell;
        }
    }
}
