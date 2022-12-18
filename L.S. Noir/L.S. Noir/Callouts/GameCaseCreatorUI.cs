using LSNoir.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using LSNoir.Common.Ped;

namespace LSNoir.Callouts
{
    internal class GameCaseCreatorUI
    {
        internal static Stage _tempStage = new Stage();

        private const Keys KeyBinding = Keys.F7;

        internal static UIMenu CaseCreatorMenu;
        private static UIMenuItem _editButton, _addButton, _removeButton;
        private static UIMenuCheckboxItem _waypointCheckbox;

        internal static void Intialize()
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(Intialize), $"Initialize");
            CaseCreatorMenu = new UIMenu("Case Editor", "Case editor for LS Noir");
            MenuHandler.AddMenu(CaseCreatorMenu);
            EditMenu.Initialize();

            CaseCreatorMenu.OnItemSelect += _menu_OnItemSelect;

            GameFiber.StartNew(ProcessMenus);
        }

        [ConsoleCommand]
        private static void LoadCaseEditor(string caseName)
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(LoadCaseEditor), $"Loading case {caseName}");
            var loadedCase = Main.InternalCaseManager.GetCase(caseName);
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(LoadCaseEditor), $"Case null: {loadedCase == null}");
            if (loadedCase == null) return;
            CaseCreatorMenu.Clear();
            AddItems();
            Game.DisplayNotification("Press ~y~F7~w~ to open the creator menu as needed");
        }

        private static void _menu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (selectedItem == _addButton)
            {
                var item = new SceneItem();
                item.ID = EditMenu.GetText("ID");
                item.InitializeNew();
                _tempStage.SceneItems.Add(item);
                CaseCreatorMenu.AddItem(AddItemToMenu(item));
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

                var marker = new Marker(item.SpawnPosition.Position, color, MarkerScale,
                    item.SpawnPosition.Rotation, 255, Marker.MarkerTypes.MarkerTypeHorizontalCircleSkinny_Arrow);
                MarkerList.Add(item, marker);
            }
            else if (selectedItem == _removeButton)
            {
                var item = _tempStage.SceneItems.First(f => f.ID == selectedItem.Text);
                CaseCreatorMenu.RemoveItemAt(_tempStage.SceneItems.IndexOf(item));
                _tempStage.SceneItems.Remove(item);
                if (MarkerList.ContainsKey(item)) MarkerList.Remove(item);
            }
            else if (selectedItem != _waypointCheckbox)
            {
                var item = _tempStage.SceneItems.First(f => f.ID == selectedItem.Text);
                Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(_menu_OnItemSelect), $"Selected item: {item.ID}");
                EditMenu.EditItem(_tempStage.SceneItems.IndexOf(item), item);
                CaseCreatorMenu.Visible = false;
            }
        }

        internal static void EditItemReturned(int index, SceneItem item)
        {
            _tempStage.SceneItems[index] = item;
            CaseCreatorMenu.RemoveItemAt(index + 3);
            CaseCreatorMenu.AddItem(AddItemToMenu(item), index + 3);
        }

        private static void AddItems()
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(AddItems), $"Adding items");
            _addButton = new UIMenuItem("Add Item", "Add a new Scene Item");
            CaseCreatorMenu.AddItem(_addButton);
            _removeButton = (new UIMenuItem("Remove Item", "Add a new Scene Item"));
            CaseCreatorMenu.AddItem(_removeButton);
            _waypointCheckbox = new UIMenuCheckboxItem("Display Waypoints", false, "Enables waypoint markers for items");
            CaseCreatorMenu.AddItem(_waypointCheckbox);

            foreach (var item in _tempStage.SceneItems)
            {
                CaseCreatorMenu.AddItem(AddItemToMenu(item));
            }
        }

        private static UIMenuItem AddItemToMenu(SceneItem item)
        {
            Logger.LogDebug(nameof(GameCaseCreatorUI), nameof(AddItems), $"Adding item {item.ID}");
            return new UIMenuItem(item.ID);
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

                if (_waypointCheckbox?.Checked == true)
                {
                    DrawWaypoints();
                }
            }
        }

        private static readonly Vector3 MarkerScale = new Vector3(0.5f, 0.5f, 0.5f);

        private static readonly Dictionary<SceneItem, Marker> MarkerList = new Dictionary<SceneItem, Marker>();

        private static void DrawWaypoints()
        {
            var list = _tempStage?.SceneItems?.ToList();
            if (list == null) return;
            
            foreach (var item in MarkerList.Values.ToList())
            {
                item.DrawMarker();
            }
        }
    }

    internal class EditMenu
    {
        private static SceneItem _item;
        private static int _menuIndex;

        private static readonly UIMenu EditMenuItem = new UIMenu("Item Editor", "Editor for scene items");

        private static UIMenuItem _id, _model, _spawnPosition, _removeScenario;
        private static UIMenuCheckboxItem _spawnPositionRotation, _vehicleLightsOn, _objectPositionFrozen;
        private static UIMenuListItem _weaponName, _addScenario, _type, _scenarioList;

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
            GameCaseCreatorUI.EditItemReturned(_menuIndex, _item);
            GameCaseCreatorUI.CaseCreatorMenu.Visible = true;
        }

        internal static void EditItem(int menuIndex, SceneItem item)
        {
            _menuIndex = menuIndex;
            _item = item;
            EditMenuItem.Clear();
            EditMenuItem.Visible = true;

            EditMenuItem.AddItem(new UIMenuItem("Unique ID", "Unique ID for item") { Enabled = false });
            _id = CreateItem(item.ID, "Press ENTER to change text");
            EditMenuItem.AddItem(new UIMenuItem("Model", "Model name for item") { Enabled = false });
            _model = CreateItem(item.Model, "Press ENTER to change text");
            _spawnPosition = CreateItem("Spawn Position", "Spawn position for item");
            EditMenuItem.AddItem(new UIMenuItem("Weapon", "Weapon model string, only needed if want weapon equipped") { Enabled = false });

            var weaponHashList = Enum.GetValues(typeof(WeaponHash)).Cast<WeaponHash>().ToList();

            var weaponStringList = new List<string>()
            {
                "None"
            };
            weaponStringList.AddRange(weaponHashList.Select(weapon => weapon.ToString()));

            _weaponName = CreateItem("Weapon", "Weapon Type", weaponStringList) as UIMenuListItem;

            _spawnPositionRotation = CreateItem("Use Rotation", "Use rotation values instead of heading for spawn position", false) as UIMenuCheckboxItem;
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
        }

        private static void _editMenu_OnListChange(UIMenu sender, UIMenuListItem listItem, int newIndex)
        {
            if (listItem != _type) return;
            switch (_type.Text)
            {
                case "Ped":
                    _weaponName.Enabled = true;
                    _vehicleLightsOn.Enabled = false;
                    _objectPositionFrozen.Enabled = false;
                    _addScenario.Enabled = true;
                    _removeScenario.Enabled = true;
                    _scenarioList.Enabled = true;
                    break;
                case "Vehicle":
                    _weaponName.Enabled = false;
                    _vehicleLightsOn.Enabled = true;
                    _objectPositionFrozen.Enabled = true;
                    _addScenario.Enabled = false;
                    _removeScenario.Enabled = false;
                    _scenarioList.Enabled = false;
                    break;
                case "Object":
                    _weaponName.Enabled = false;
                    _vehicleLightsOn.Enabled = false;
                    _objectPositionFrozen.Enabled = true;
                    _addScenario.Enabled = false;
                    _removeScenario.Enabled = false;
                    _scenarioList.Enabled = false;
                    break;
            }
        }

        private static void _editMenu_OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool @checked)
        {
            if (checkboxItem == _vehicleLightsOn) _item.VehicleLightsOn = @checked;
            else if (checkboxItem == _objectPositionFrozen) _item.ObjectPositionFrozen = @checked;
        }

        private static void _editMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (selectedItem == _spawnPosition)
            {
                _item.SpawnPosition = new CaseManager.Resources.SpawnPoint()
                {
                    Position = Game.LocalPlayer.Character.Position
                };
                if (_spawnPositionRotation.Checked) _item.SpawnPosition.Rotation = Game.LocalPlayer.Character.Rotation;
                else _item.SpawnPosition.Heading = Game.LocalPlayer.Character.Heading;
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
                _model.Text = result;
                _item.Model = result;
            }
            else if (selectedItem == _addScenario)
            {
                var result = _addScenario.SelectedItem.Value.ToString();
                _item.Scenarios.Add(result);
                _scenarioList.Collection.Add(result);
            }
        }

        internal static string GetText(string header)
        {
            return OnscreenTextbox.DisplayBox(100, header, ""); ;
        }

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
