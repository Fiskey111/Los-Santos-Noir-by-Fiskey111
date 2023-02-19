using System;
using System.Collections.Generic;
using System.Linq;
using CaseManager.NewData;
using CaseManager.Resources;
using LSNoir.Common;
using LSNoir.Common.Ped;
using LSNoir.Common.UI;
using LSNoir.Extensions;
using LSPD_First_Response.Engine.Scripting.Entities;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace LSNoir.Menus
{
    public class SceneItemEditor
    {
        private static SceneItem _item;
        private static int _menuIndex;
        private static string _oldId;

        private static readonly UIMenu EditMenuItem = new UIMenu("Item Editor", "Editor for scene items");

        private static UIMenuItem _id, _model, _spawnPosition, _removeScenario;
        private static UIMenuCheckboxItem _randomModel, _vehicleLightsOn, _objectPositionFrozen, _interactionEntity, _secondarySpawnPosition, _useRotation;
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
            if (_randomModel.Checked) _item.Model = Model.PedModels.PickRandom().Name;
            StageEditor.EditItemReturned(_menuIndex, _item, _oldId);
            StageEditor.CaseCreatorMenu.Visible = true;
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
            _recentModelList = CreateItem("Use Existing Model",
                "Press ENTER to autofill the current model name for this item", StageEditor.TempStage.SceneItems.Select(i => i.Model).Distinct().ToList()) as UIMenuListItem;
            _randomModel = CreateItem("Random Model", "If selected, the model will be randomized (peds and vehicles only)", false) as UIMenuCheckboxItem;
            
            _interactionEntity = CreateItem("Is Interactable", "Determines if the entity has dialog", false) as UIMenuCheckboxItem;
            
            _spawnPosition = CreateItem("Spawn Position", "Spawn position for item");
            _useRotation = CreateItem("Use Rotation", "Uses rotation instead of heading when spawning", false) as UIMenuCheckboxItem;
            _secondarySpawnPosition = CreateItem("Secondary Spawn Point",
                "Secondary spawn point for meeting for pickups or other things (set when checked)", false) as UIMenuCheckboxItem;
            _secondarySpawnPosition.Enabled = _interactionEntity.Checked;
            
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
                var result = _weaponName.SelectedItem.Value.ToString();
                Logger.LogDebug(nameof(SceneItemEditor), nameof(_editMenu_OnListChange), $"Weapon selected: {result} | index {newIndex}");
                var valid = Enum.TryParse<WeaponHash>(result, out var weaponHash);
                _item.WeaponName = !valid ? string.Empty : ((uint)weaponHash).ToString();
            }
        }

        private static void _editMenu_OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool @checked)
        {
            if (checkboxItem == _randomModel) _item.ModelRandom = @checked;
            else if (checkboxItem == _vehicleLightsOn) _item.VehicleLightsOn = @checked;
            else if (checkboxItem == _objectPositionFrozen) _item.ObjectPositionFrozen = @checked;
            else if (checkboxItem == _secondarySpawnPosition)
            {
                _item.InteractionSettings.SecondaryPosition = Game.LocalPlayer.GetPlayerSpawnpoint();
            }
            else if (checkboxItem == _interactionEntity)
            {
                _item.IsInteractable = @checked;
                _secondarySpawnPosition.Enabled = _interactionEntity.Checked;
            }
        }

        private static void _editMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (selectedItem == _spawnPosition)
            {
                _item.SpawnPosition = new SpawnPoint
                {
                    Position = Game.LocalPlayer.Character.Position,
                    Rotation = Rotator.Zero,
                    Heading = Game.LocalPlayer.Character.Heading
                };
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
                var result = OnscreenTextbox.GetTextFromTextboxStandard("Unique ID");
                _id.Text = result;
                _item.ID = result;
            }
            else if (selectedItem == _model)
            {
                var result = OnscreenTextbox.GetTextFromTextboxStandard("Model Name");
                if (!new Model(result).IsValid)
                {
                    Game.DisplayNotification($"Model {result} is not valid");
                    return;
                }
                _model.Text = result;
                _item.Model = result;
            }
            else if (selectedItem == _recentModelList)
            {
                _model.Text = _recentModelList.SelectedItem.DisplayText;
                _item.Model = _recentModelList.SelectedItem.DisplayText;
            }
            else if (selectedItem == _addScenario)
            {
                var result = _addScenario.SelectedItem.DisplayText;
                _item.Scenarios.Add(result);
                _scenarioList.Collection.Add(result);
            }
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