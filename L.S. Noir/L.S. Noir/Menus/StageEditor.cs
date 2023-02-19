using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CaseManager.NewData;
using CaseManager.Resources;
using LSNoir.Callouts.Universal;
using LSNoir.Common;
using LSNoir.Common.IO;
using LSNoir.Common.Ped;
using LSNoir.Common.UI;
using Rage;
using Rage.Attributes;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

namespace LSNoir.Menus
{
    internal static class StageEditor
    {
        internal static Stage TempStage = new Stage();

        private const Keys KeyBinding = Keys.F7;
        private const string FileDirectory = @"Plugins\LSPDFR\LSNoir\Development";

        internal static UIMenu CaseCreatorMenu;
        private static UIMenuItem _addButton, _removeButton, _exportScene, _sceneId, _loadFile;
        private static UIMenuListItem _stageType;
        
        private static UIMenuCheckboxItem _waypointCheckbox, _spawnItems;

        [ConsoleCommand]
        private static void EnableCaseEditor()
        {
            if (CaseCreatorMenu == null) Initialize(); 
            Game.DisplayNotification("Press ~y~F7~w~ to open the creator menu as needed");
        }
        
        [ConsoleCommand]
        private static void ResetCaseEditor()
        {
            CaseCreatorMenu = null;
            if (CaseCreatorMenu == null) Initialize(); 
            Game.DisplayNotification("Menu reset. Press ~y~F7~w~ to open the creator menu as needed");
        }

        internal static void Initialize()
        {
            Logger.LogDebug(nameof(StageEditor), nameof(Initialize), $"Initialize");
            CaseCreatorMenu = new UIMenu("Stage Editor", "Stage editor for LS Noir");
            MenuHandler.AddMenu(CaseCreatorMenu);
            
            SceneItemEditor.Initialize();
            
            AddItems();

            CaseCreatorMenu.OnItemSelect += _menu_OnItemSelect;
            CaseCreatorMenu.OnCheckboxChange += CaseCreatorMenuOnOnCheckboxChange;
            CaseCreatorMenu.OnListChange += CaseCreatorMenuOnOnListChange;

            GameFiber.StartNew(ProcessMenus);
            GameFiber.StartNew(DisplayFiber);
        }

        private static void CaseCreatorMenuOnOnListChange(UIMenu sender, UIMenuListItem listitem, int newindex)
        {
            if (listitem == _stageType)
            {
                TempStage.StageType = listitem.SelectedItem.Value.ToString();
            }
        }

        private static void CaseCreatorMenuOnOnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxitem, bool @checked)
        {
            if (checkboxitem == _spawnItems)
            {
                if (@checked)
                {
                    Game.DisplayNotification("Spawning items in 5 seconds...\nBetter get out of the way!!");
                    GameFiber.Sleep(5000);
                    foreach (var item in TempStage.SceneItems)
                    {
                        Logger.LogDebug(nameof(StageEditor), nameof(CaseCreatorMenuOnOnCheckboxChange), $"Spawning entity: {item.ID}");
                        item.SpawnItem(out var ent);
                        TempEntityList.Add(ent);
                    }
                }
                else
                {
                    foreach (var ent in TempEntityList.Where(ent => ent))
                    {
                        ent.Delete();
                    }
                    TempEntityList.Clear();
                }
            }
        }

        private static readonly List<Entity> TempEntityList = new List<Entity>();


        private static void AddItems()
        {
            Logger.LogDebug(nameof(StageEditor), nameof(AddItems), $"Adding items");
            _sceneId = new UIMenuItem("Scene ID: None", "Sets the scene ID (used in file naming, not necessary)");
            CaseCreatorMenu.AddItem(_sceneId);
            _stageType = new UIMenuListItem("Stage Type", "The stage type for this method to use when playing", StageTypesForCases.StageTypeList);
            CaseCreatorMenu.AddItem(_stageType);
            _addButton = new UIMenuItem("Add Item", "Add a new Scene Item");
            CaseCreatorMenu.AddItem(_addButton);
            _removeButton = (new UIMenuItem("Remove Item", "Add a new Scene Item"));
            CaseCreatorMenu.AddItem(_removeButton);
            _waypointCheckbox = new UIMenuCheckboxItem("Display Waypoints", false, "Enables waypoint markers for items");
            CaseCreatorMenu.AddItem(_waypointCheckbox);
            _exportScene = new UIMenuItem("Export Scene", "Exports the scene to a json file in the LS Noir folder");
            CaseCreatorMenu.AddItem(_exportScene);
            _loadFile = new UIMenuItem("Load Existing Scene",
                "Load file from file Plugins/LSPDFR/LSNoir/Development/loadScene.json");
            CaseCreatorMenu.AddItem(_loadFile);
            _spawnItems = new UIMenuCheckboxItem("Spawn Items", false, "When checked, all scene items will be spawned in their positions");
            CaseCreatorMenu.AddItem(_spawnItems);
        }

        private static void _menu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                _loadFile.Enabled = TempStage.SceneItems.Count <= 0;

                if (selectedItem == _addButton)
                {
                    var id = OnscreenTextbox.GetTextFromTextboxStandard("ID");
                    if (string.IsNullOrWhiteSpace(id)) return;
                    if (TempStage.SceneItems.Any(i => i.ID == id))
                    {
                        Game.DisplayNotification($"~r~Error~w~: ID ~y~{id}~w~ already in use.  IDs must be unique.");
                        return;
                    }

                    var item = new SceneItem();
                    item.InitializeNew();
                    item.ID = id;
                    SetPosition(item);
                    TempStage.SceneItems.Add(item);
                    CaseCreatorMenu.AddItem(AddItemToMenu(item));
                    MarkerList.Add(item, GetMarkerForItem(item));
                    OpenItemEditor(item);
                }
                else if (selectedItem == _removeButton)
                {
                    var closestItem =
                        TempStage.SceneItems
                            .OrderBy(i => i.SpawnPosition.Position.DistanceTo(Game.LocalPlayer.Character))
                            .FirstOrDefault();
                    if (closestItem == null ||
                        closestItem.SpawnPosition.Position.DistanceTo(Game.LocalPlayer.Character) > 3f)
                    {
                        Game.DisplaySubtitle($"~r~No item found close");
                        return;
                    }

                    if (MarkerList.ContainsKey(closestItem)) MarkerList.Remove(closestItem);
                    if (_scaleforms.ContainsKey(closestItem)) _scaleforms.Remove(closestItem);
                    TempStage.SceneItems.Remove(closestItem);
                    var itemInMenu = CaseCreatorMenu.MenuItems.First(f => f.Text == closestItem.ID);
                    CaseCreatorMenu.RemoveItemAt(CaseCreatorMenu.MenuItems.IndexOf(itemInMenu));
                    CaseCreatorMenu.RefreshIndex();
                }
                else if (selectedItem == _exportScene)
                {
                    ExportScene();
                }
                else if (selectedItem == _sceneId)
                {
                    var text = OnscreenTextbox.GetTextFromTextboxStandard("Stage ID");
                    if (string.IsNullOrWhiteSpace(text)) return;
                    _sceneId.Text = $"Scene ID: {text}";
                    TempStage.ID = text;
                }
                else if (selectedItem == _loadFile)
                {
                    var stage = JsonHelper.ReadFileJson<Stage>($@"{FileDirectory}/loadScene.json");
                    if (stage?.SceneItems == null || stage.SceneItems.Count < 1)
                    {
                        Game.DisplayNotification($"~r~Error loading");
                        Logger.LogDebug(nameof(StageEditor), nameof(_menu_OnItemSelect),
                            $"Stage == null {stage == null}");
                        Logger.LogDebug(nameof(StageEditor), nameof(_menu_OnItemSelect),
                            $"SceneItems == null {stage?.SceneItems == null}");
                        Logger.LogDebug(nameof(StageEditor), nameof(_menu_OnItemSelect),
                            $"SceneItems.Count {stage?.SceneItems?.Count}");
                        return;
                    }
                    TempStage = stage;
                    
                    LoadItems();
                }
                else if (selectedItem == _stageType)
                {
                    TempStage.StageType = _stageType.SelectedValue.ToString();
                }
                else if (selectedItem != _waypointCheckbox)
                {
                    OpenItemEditor(TempStage.SceneItems.First(f => f.ID == selectedItem.Text));
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(nameof(StageEditor), nameof(_menu_OnItemSelect), ex.ToString());
            }
        }

        private static void OpenItemEditor(SceneItem item)
        {
            Logger.LogDebug(nameof(StageEditor), nameof(_menu_OnItemSelect), $"Selected item: {item.ID}");
            SceneItemEditor.EditItem(TempStage.SceneItems.IndexOf(item), ref item);
            CaseCreatorMenu.Visible = false;
        }

        private static void SetPosition(SceneItem item)
        {
            item.SpawnPosition =
                new SpawnPoint(Game.LocalPlayer.Character.Heading, Game.LocalPlayer.Character.Position)
                {
                    Rotation = new Rotator(Game.LocalPlayer.Character.Rotation.Pitch - 180f, Game.LocalPlayer.Character.Rotation.Roll, Game.LocalPlayer.Character.Rotation.Yaw)
                };
        }

        private static Marker GetMarkerForItem(SceneItem item)
        {
            Color color;
            switch (item.Type)
            {
                case SceneItem.EItemType.Object:
                    color = Color.Blue;
                    break;
                case SceneItem.EItemType.Ped:
                    color = Color.Red;
                    break;
                case SceneItem.EItemType.Vehicle:
                    color = Color.Orange;
                    break;
                default:
                    color = Color.Green;
                    break;
            }

            var heading = item.SpawnPosition.Heading;
            return new Marker(item.SpawnPosition.Position, color, MarkerScale,
                new Rotator(0f, 0, heading), 255, Marker.MarkerTypes.MarkerTypeHorizontalCircleSkinny_Arrow);
        }
        
        
        internal static void EditItemReturned(int index, SceneItem item, string oldId)
        {
            TempStage.SceneItems[index] = item;
            if (oldId != item.ID && CaseCreatorMenu.MenuItems.Any(i => i.Text == oldId))
            {
                CaseCreatorMenu.MenuItems.First(q => q.Text == oldId).Text = item.ID;
            }
            // CaseCreatorMenu.RemoveItemAt(index + 3);
            // CaseCreatorMenu.AddItem(AddItemToMenu(item), index + 3);
            MarkerList[item] = GetMarkerForItem(item);
            var scaleform = new Scaleform();
            scaleform.Load("MP_MISSION_NAME_FREEMODE");
            if (!_scaleforms.ContainsKey(item)) _scaleforms.Add(item, scaleform);
            if (_spawnItems.Checked)
            {
                Game.DisplayNotification("Spawning entity in 5 seconds, better move!");
                GameFiber.Sleep(5000);
                item.SpawnItem(out var ent);
                TempEntityList.Add(ent);
            }
        }

        private static UIMenuItem AddItemToMenu(SceneItem item)
        {
            Logger.LogDebug(nameof(StageEditor), nameof(AddItems), $"Adding item {item.ID}");
            return new UIMenuItem(item.ID);
        }

        internal static void ExportScene()
        {
            try
            {
                Logger.LogDebug(nameof(StageEditor), nameof(ExportScene), "Exporting scene");
                if (TempStage == null || TempStage.SceneItems.Count < 1)
                {
                    Logger.LogDebug(nameof(StageEditor), nameof(ExportScene), "Temp stage is null or no items");
                    Game.DisplayNotification($"~r~No stage or scene items present");
                    return;
                }

                var valid = JsonHelper.SaveFileJson(
                    $"{FileDirectory}\\{_sceneId.Text.Remove(0, 10)}_GameCaseCreator_{DateTime.Now:yyMMdd_hhmmss}.json",
                    TempStage);

                var success = valid ? "~g~succesfully" : "~r~unsuccesfully";

                Game.DisplayNotification($"File saved {valid}");
            }
            catch (Exception ex)
            {
                Logger.LogDebug(nameof(StageEditor), nameof(ExportScene), ex.ToString());
            }
        }

        private static void ProcessMenus()
        {
            while (true)
            {
                GameFiber.Yield();

                MenuHandler.DrawMenus();

                if (Game.IsKeyDown(KeyBinding) && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    CaseCreatorMenu.Visible = !CaseCreatorMenu.Visible;
                }
            }
        }

        private static void LoadItems()
        {
            Logger.LogDebug(nameof(StageEditor), nameof(LoadItems), $"Loading items from existing scene. Total items: {TempStage.SceneItems.Count}");
            foreach (var item in TempStage.SceneItems)
            {
                MarkerList[item] = GetMarkerForItem(item);
                var scaleform = new Scaleform();
                scaleform.Load("MP_MISSION_NAME_FREEMODE");
                _scaleforms.Add(item, scaleform);

                CaseCreatorMenu.AddItem(AddItemToMenu(item));
            }
        }

        private static void DisplayFiber()
        {
            while (true)
            {
                if (_waypointCheckbox?.Checked == true)
                {
                    DrawWaypoints();

                    DisplayHelp();
                }
                GameFiber.Yield();
            }
        }
        
        private static readonly Vector3 MarkerScale = new Vector3(0.5f, 0.5f, 0.5f);

        private static readonly Dictionary<SceneItem, Marker> MarkerList = new Dictionary<SceneItem, Marker>();

        private static void DrawWaypoints()
        {
            var list = TempStage?.SceneItems?.ToList();
            if (list == null) return;

            for (var index = 0; index < MarkerList.Values.ToList().Count; index++)
            {
                var item = MarkerList.Values.ToList()[index];
                item.DrawMarker();
            }
        }

        private static SceneItem _closestItem;
        private static Dictionary<SceneItem, Scaleform> _scaleforms = new Dictionary<SceneItem, Scaleform>();
        private static readonly Vector3 Scale = new Vector3(2f, 2f, 2f);

        private static void DisplayHelp()
        {
            var closestItem = _scaleforms.Keys.ToList()
                .OrderBy(i => i.SpawnPosition.Position.DistanceTo(Game.LocalPlayer.Character)).FirstOrDefault();

            if (closestItem == null) return;

            var scaleform = _scaleforms[closestItem];
            scaleform.CallFunction("SET_MISSION_INFO", $"{closestItem.ID}", "", "", "", "", false, "", "", "", "");
            scaleform.Render3D(new Vector3(closestItem.SpawnPosition.Position.X, closestItem.SpawnPosition.Position.Y, closestItem.SpawnPosition.Position.Z - 0.2f), 
                new Vector3(0f, closestItem.SpawnPosition.Heading, 0f), Scale);
        }
    }
}
