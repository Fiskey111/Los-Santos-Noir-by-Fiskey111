using LSNoir.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RAGENativeUI.PauseMenu;
using Rage.Attributes;
using Rage;
using RAGENativeUI.Elements;
using RAGENativeUI;
using LSNoir.Common.UI;
using CaseManager.NewData;
using System.Windows.Forms;
using CaseManager.Resources;
using LSNoir.Common.IO;
using LSNoir.Common.Ped;
using Newtonsoft.Json;
using Rage.Native;
using Graphics = Rage.Graphics;
using Object = Rage.Object;

namespace LSNoir.Callouts
{
    internal class GameCaseCreatorUI
    {
        internal static Stage _tempStage = new Stage();

        private const Keys KeyBinding = Keys.F7;
        private const string FileDirectory = @"Plugins\LSPDFR\LSNoir\Development";

        internal static UIMenu CaseCreatorMenu;
        private static UIMenuItem _editButton, _addButton, _removeButton, _exportScene, _sceneID, _loadFile;
        private static UIMenuCheckboxItem _waypointCheckbox;

        internal static void Initialize()
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(Initialize), $"Initialize");
            CaseCreatorMenu = new UIMenu("Scene Editor", "Scene editor for LS Noir");
            MenuHandler.AddMenu(CaseCreatorMenu);
            EditMenu.Initialize();
            AddItems();

            CaseCreatorMenu.OnItemSelect += _menu_OnItemSelect;

            GameFiber.StartNew(ProcessMenus);
            GameFiber.StartNew(DisplayFiber);
        }

        [ConsoleCommand]
        private static void EnableCaseEditor()
        {
            if (CaseCreatorMenu == null) Initialize(); 
            Game.DisplayNotification("Press ~y~F7~w~ to open the creator menu as needed");
        }

        private static void _menu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                _loadFile.Enabled = _tempStage.SceneItems?.Count <= 0;

                if (selectedItem == _addButton)
                {
                    var id = EditMenu.GetText("ID");
                    if (string.IsNullOrWhiteSpace(id)) return;
                    if (_tempStage.SceneItems.Any(i => i.ID == id))
                    {
                        Game.DisplayNotification($"~r~Error~w~: ID ~y~{id}~w~ already in use.  IDs must be unique.");
                        return;
                    }

                    var item = new SceneItem();
                    item.InitializeNew();
                    item.ID = id;
                    SetPosition(item);
                    _tempStage.SceneItems.Add(item);
                    CaseCreatorMenu.AddItem(AddItemToMenu(item));
                    MarkerList.Add(item, GetMarkerForItem(item));
                    OpenItemEditor(item);
                }
                else if (selectedItem == _removeButton)
                {
                    var closestItem =
                        _tempStage.SceneItems
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
                    _tempStage.SceneItems.Remove(closestItem);
                    var itemInMenu = CaseCreatorMenu.MenuItems.First(f => f.Text == closestItem.ID);
                    CaseCreatorMenu.RemoveItemAt(CaseCreatorMenu.MenuItems.IndexOf(itemInMenu));
                    CaseCreatorMenu.RefreshIndex();
                }
                else if (selectedItem == _exportScene)
                {
                    ExportScene();
                }
                else if (selectedItem == _sceneID)
                {
                    var text = EditMenu.GetText("Stage ID");
                    if (string.IsNullOrWhiteSpace(text)) return;
                    _sceneID.Text = $"Scene ID: {text}";
                }
                else if (selectedItem == _loadFile)
                {
                    var stage = JsonHelper.ReadFileJson<Stage>($@"{FileDirectory}/loadScene.json");
                    if (stage?.SceneItems == null || stage.SceneItems.Count < 1)
                    {
                        Game.DisplayNotification($"~r~Error loading");
                        Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect),
                            $"Stage == null {stage == null}");
                        Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect),
                            $"SceneItems == null {stage?.SceneItems == null}");
                        Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect),
                            $"SceneItems.Count {stage?.SceneItems?.Count}");
                        return;
                    }
                    _tempStage = stage;
                    
                    LoadItems();
                }
                else if (selectedItem != _waypointCheckbox)
                {
                    OpenItemEditor(_tempStage.SceneItems.First(f => f.ID == selectedItem.Text));
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect), ex.ToString());
            }
        }

        private static void OpenItemEditor(SceneItem item)
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect), $"Selected item: {item.ID}");
            EditMenu.EditItem(_tempStage.SceneItems.IndexOf(item), ref item);
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
            _tempStage.SceneItems[index] = item;
            if (oldId != item.ID && CaseCreatorMenu.MenuItems.Any(i => i.Text == oldId))
            {
                CaseCreatorMenu.MenuItems.First(q => q.Text == oldId).Text = item.ID;
            }
            // CaseCreatorMenu.RemoveItemAt(index + 3);
            // CaseCreatorMenu.AddItem(AddItemToMenu(item), index + 3);
            MarkerList[item] = GetMarkerForItem(item);
            var scaleform = new Scaleform();
            scaleform.Load("MP_MISSION_NAME_FREEMODE");
            _scaleforms.Add(item, scaleform);
        }

        private static void AddItems()
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(AddItems), $"Adding items");
            _sceneID = new UIMenuItem("Scene ID: None", "Sets the scene ID (used in file naming, not necessary)");
            CaseCreatorMenu.AddItem(_sceneID);
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
        }

        private static UIMenuItem AddItemToMenu(SceneItem item)
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(AddItems), $"Adding item {item.ID}");
            return new UIMenuItem(item.ID);
        }

        internal static void ExportScene()
        {
            try
            {
                Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(ExportScene), "Exporting scene");
                if (_tempStage == null || _tempStage.SceneItems.Count < 1)
                {
                    Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(ExportScene), "Temp stage is null or no items");
                    Game.DisplayNotification($"~r~No stage or scene items present");
                    return;
                }

                var valid = JsonHelper.SaveFileJson(
                    $"{FileDirectory}\\{_sceneID.Text.Remove(0, 10)}_GameCaseCreator_{DateTime.Now:yyMMdd_hhmmss}.json",
                    _tempStage);

                var success = valid ? "~g~succesfully" : "~r~unsuccesfully";

                Game.DisplayNotification($"File saved {valid}");
            }
            catch (Exception ex)
            {
                Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(ExportScene), ex.ToString());
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
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(LoadItems), $"Loading items from existing scene. Total items: {_tempStage.SceneItems.Count}");
            foreach (var item in _tempStage.SceneItems)
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
            var list = _tempStage?.SceneItems?.ToList();
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

    internal class EditMenu
    {
        private static SceneItem _item;
        private static int _menuIndex;
        private static string _oldId;
        private static string _lastModel = string.Empty;

        private static readonly UIMenu EditMenuItem = new UIMenu("Item Editor", "Editor for scene items");

        private static UIMenuItem _id, _model, _spawnPosition, _removeScenario;
        private static UIMenuCheckboxItem _randomModel, _vehicleLightsOn, _objectPositionFrozen, _lastModelCheckbox;
        private static UIMenuListItem _weaponName, _addScenario, _type, _scenarioList, _recentModelList;

        internal static void Initialize()
        {
            MenuHandler.AddMenu(EditMenuItem);
            EditMenuItem.Width = 0.25f;

            EditMenuItem.OnItemSelect += _editMenu_OnItemSelect;
            EditMenuItem.OnCheckboxChange += _editMenu_OnCheckboxChange;
            EditMenuItem.OnListChange += _editMenu_OnListChange;
            EditMenuItem.OnMenuClose += EditMenuItem_OnMenuClose;
        }

        private static void EditMenuItem_OnMenuClose(UIMenu sender)
        {
            EditMenuItem.Visible = false;
            GameCaseCreatorUI.EditItemReturned(_menuIndex, _item, _oldId);
            GameCaseCreatorUI.CaseCreatorMenu.Visible = true;
            EditMenuItem.MouseEdgeEnabled = false;
        }

        internal static void EditItem(int menuIndex, ref SceneItem item)
        {
            _menuIndex = menuIndex;
            _item = item;
            _oldId = item.ID;
            EditMenuItem.Clear();
            EditMenuItem.Visible = true;

            EditMenuItem.AddItem(new UIMenuItem("Unique ID", "Unique ID for item") { Skipped = false });
            _id = CreateItem(item.ID, "Press ENTER to change text");
            EditMenuItem.AddItem(new UIMenuItem("Model", "Model name for item") { Skipped = false });
            _model = CreateItem(item.Model, "Press ENTER to change text");
            _lastModelCheckbox = CreateItem("Use Last Model",
                "If checked, the last model name will be used for this item", false) as UIMenuCheckboxItem;
            _randomModel = CreateItem("Random Model", "If selected, the model will be randomized (peds and vehicles only)", false) as UIMenuCheckboxItem;
            _spawnPosition = CreateItem("Spawn Position", "Spawn position for item");
            EditMenuItem.AddItem(new UIMenuItem("Weapon", "Weapon model string, only needed if want weapon equipped") { Skipped = false });

            var weaponHashList = Enum.GetValues(typeof(WeaponHash)).Cast<WeaponHash>().ToList();

            var weaponStringList = new List<string>()
            {
                "None"
            };
            weaponStringList.AddRange(weaponHashList.Select(weapon => weapon.ToString()));

            _weaponName = CreateItem("Weapon", "Weapon Type", weaponStringList) as UIMenuListItem;

            _vehicleLightsOn = CreateItem("Vehicle Lights", "Vehicle lights activated", item.VehicleLightsOn) as UIMenuCheckboxItem;
            _objectPositionFrozen = CreateItem("Freeze Object Position", "Freeze position of object", item.ObjectPositionFrozen) as UIMenuCheckboxItem;

            var enums = Enum.GetValues(typeof(SceneItem.EItemType)).Cast<SceneItem.EItemType>().ToList();

            var enumStringList = (from enumValue in enums
                                           select enumValue.ToString()).ToList();
            _type = CreateItem("Object Type", "Type of object", enumStringList) as UIMenuListItem;

            _scenarioList = CreateItem("Scenarios", "List of scenarios for ped to use", item.Scenarios) as UIMenuListItem;
            _addScenario = CreateItem("Add Scenario", "Add the selected scenario", 
                (from enumValue in Enum.GetValues(typeof(PedScenario.ScenarioList)).Cast<PedScenario.ScenarioList>().ToList() select enumValue.ToString()).ToList()) as UIMenuListItem;
            _removeScenario = CreateItem("Remove Scenario", "Remove currently selected scenario");

            _editMenu_OnListChange(EditMenuItem, _type, 0);
        }

        private static void _editMenu_OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
        {
            if (sender != EditMenuItem) return;

            if (listItem == _type)
            {
                switch (_type.SelectedItem.Value.ToString())
                {
                    case "Ped":
                        _weaponName.Enabled = true;
                        _vehicleLightsOn.Enabled = false;
                        _objectPositionFrozen.Enabled = false;
                        _addScenario.Enabled = true;
                        _removeScenario.Enabled = true;
                        _scenarioList.Enabled = true;
                        _item.Type = SceneItem.EItemType.Ped;
                        break;
                    case "Vehicle":
                        _weaponName.Enabled = false;
                        _vehicleLightsOn.Enabled = true;
                        _objectPositionFrozen.Enabled = true;
                        _addScenario.Enabled = false;
                        _removeScenario.Enabled = false;
                        _scenarioList.Enabled = false;
                        _item.Type = SceneItem.EItemType.Vehicle;
                        break;
                    case "Object":
                        _weaponName.Enabled = false;
                        _vehicleLightsOn.Enabled = false;
                        _objectPositionFrozen.Enabled = true;
                        _addScenario.Enabled = false;
                        _removeScenario.Enabled = false;
                        _scenarioList.Enabled = false;
                        _item.Type = SceneItem.EItemType.Object;
                        break;
                }
            }
            else if (listItem == _weaponName)
            {
                if (_weaponName.SelectedItem.Value.ToString() == "None") return;
                _item.WeaponName = _weaponName.SelectedItem.Value.ToString();
            }
            
        }

        private static void _editMenu_OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool @checked)
        {
            if (checkboxItem == _randomModel) _item.ModelRandom = @checked;
            else if (checkboxItem == _vehicleLightsOn) _item.VehicleLightsOn = @checked;
            else if (checkboxItem == _objectPositionFrozen) _item.ObjectPositionFrozen = @checked;
            else if (checkboxItem == _lastModelCheckbox && !string.IsNullOrWhiteSpace(_lastModel)) 
            {
                _model.Text = _lastModel;
                _item.Model = _lastModel;
            }
        }

        private static void _editMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (selectedItem == _spawnPosition)
            {
                _item.SpawnPosition = new SpawnPoint()
                {
                    Position = Game.LocalPlayer.Character.Position
                };
                _item.SpawnPosition.Rotation = Game.LocalPlayer.Character.Rotation;
                _item.SpawnPosition.Heading = Game.LocalPlayer.Character.Heading;
            }
            else if (selectedItem == _removeScenario)
            {
                if (_item.Scenarios.Count < 1) return;
                var scenarioIndex = _item.Scenarios.Count - 1;
                _item.Scenarios.RemoveAt(scenarioIndex);
                _scenarioList.Collection.RemoveAt(scenarioIndex);
            }
            else if (selectedItem == _id)
            {
                var result = GetText("Unique ID");
                _id.Text = result;
                _item.ID = result;
            }
            else if (selectedItem == _model)
            {
                var result = GetText("Model Name");
                if (!new Model(result).IsValid)
                {
                    Game.DisplayNotification($"Model {result} is not valid");
                    return;
                }
                _model.Text = result;
                _item.Model = result;
                _lastModel = result;
            }
            else if (selectedItem == _addScenario)
            {
                var result = _addScenario.SelectedItem.Value.ToString();
                _item.Scenarios.Add(result);
                _scenarioList.Collection.Add(result);
            }
        }

        internal static string GetText(string header) => OnscreenTextbox.DisplayBox(100, header, "");

        private static UIMenuItem CreateItem(string text, string description)
        {
            var tempItem = new UIMenuItem(text, description);
            EditMenuItem.AddItem(tempItem);
            return tempItem;
        }

        private static UIMenuItem CreateItem(string text, string description, bool _checked)
        {
            var tempItem = new UIMenuCheckboxItem(text, _checked, description);
            EditMenuItem.AddItem(tempItem);
            return tempItem;
        }

        private static UIMenuItem CreateItem(string text, string description, List<string> items)
        {
            var tempItem = new UIMenuListItem(text, description, items.Cast<dynamic>());
            EditMenuItem.AddItem(tempItem);
            return tempItem;
        }


    }
}
