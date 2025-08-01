using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
#endif
using UnityEngine.UIElements;
#if UNITY_EDITOR
#endif


#if UNITY_EDITOR
public class BaseNode : Node
{
    public DialogStepNode DialogStepNode;
    public NodeViewer visuals;

    public BaseNode(DialogStepNode _data, bool _startNode)
    {
        DialogStepNode = _data != null ? _data : new DialogStepNode();
        visuals = new NodeViewer(this, _startNode);
    }
}

public class NodeViewer : VisualElement
{
    BaseNode node;

    public NodeViewer(BaseNode _node, bool _startNode)
    {
        node = _node;

        if (node.DialogStepNode.choices == 1)
        {
            VisualTreeAsset tree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Dialogs/VNCreator/BaseNodeTemplate.uxml");
            tree.CloneTree(this);

            styleSheets.Add(
            AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Dialogs/VNCreator/BaseNodeStyle.uss"));
            
            if (_startNode)
            {
                DropdownField dialogStyleDropdownMenu = this.Query<DropdownField>("Style_Dialogs");
                dialogStyleDropdownMenu.choices = Enum.GetNames(typeof(Enums.DialogStyle)).ToList();
                dialogStyleDropdownMenu.value = _node.DialogStepNode.styleOfDialog.ToString();

                // Добавляем обработчик события изменения значения
                dialogStyleDropdownMenu.RegisterValueChangedCallback(evt =>
                {
                    dialogStyleDropdownMenu.value = Enum.Parse(typeof(Enums.DialogStyle), evt.newValue).ToString();
                    _node.DialogStepNode.styleOfDialog =
                        (Enums.DialogStyle)Enum.Parse(typeof(Enums.DialogStyle), evt.newValue);
                });

                Toggle moreReadToggle = this.Query<Toggle>("More_Read");
                moreReadToggle.value = _node.DialogStepNode.moreRead;
                moreReadToggle.RegisterValueChangedCallback(evt =>
                {
                    _node.DialogStepNode.moreRead = moreReadToggle.value;
                });

                Toggle canMoveToggle = this.Query<Toggle>("Can_Move");
                canMoveToggle.value = _node.DialogStepNode.canMove;
                canMoveToggle.RegisterValueChangedCallback(evt => { _node.DialogStepNode.canMove = canMoveToggle.value; });

                Toggle canInterToggle = this.Query<Toggle>("Can_Inter");
                canInterToggle.value = _node.DialogStepNode.canInter;
                canInterToggle.RegisterValueChangedCallback(evt =>
                {
                    _node.DialogStepNode.canInter = canInterToggle.value;
                });

                Toggle darkAfterEndToggle = this.Query<Toggle>("Dark_End");
                darkAfterEndToggle.value = _node.DialogStepNode.darkAfterEnd;
                darkAfterEndToggle.RegisterValueChangedCallback(evt =>
                {
                    _node.DialogStepNode.darkAfterEnd = darkAfterEndToggle.value;
                });

                TextField mainPanelDelayTextField = this.Query<TextField>("MainPanelStartDelay");
                mainPanelDelayTextField.value = _node.DialogStepNode.mainPanelStartDelay.ToString();

                mainPanelDelayTextField.RegisterValueChangedCallback(evt =>
                {
                    _node.DialogStepNode.mainPanelStartDelay = float.Parse(mainPanelDelayTextField.value, CultureInfo.InvariantCulture);
                });
            }
            else
            {
                Foldout dialogSettingFoldout = this.Query<Foldout>("Dialog_Settings");
                dialogSettingFoldout.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }

            Button npcIcon = this.Query<Button>("npcIcon");
            ObjectField charNameField = this.Query<ObjectField>("Character");
            charNameField.value = (Npc)_node.DialogStepNode.character;
            charNameField.RegisterValueChangedCallback(e =>
                {
                    node.DialogStepNode.character = (Npc)charNameField.value;
                    var image = npcIcon.iconImage;
                    image.sprite = node.DialogStepNode.character.GetStyleIcon(Enums.IconMood.Standard);
                    npcIcon.iconImage = image;
                }
            );

            DropdownField moodDropdownMenu = this.Query<DropdownField>("Char_Mood");
            moodDropdownMenu.choices = Enum.GetNames(typeof(Enums.IconMood)).ToList();
            moodDropdownMenu.value = _node.DialogStepNode.mood.ToString();

            moodDropdownMenu.RegisterValueChangedCallback(evt =>
            {
                moodDropdownMenu.value = Enum.Parse(typeof(Enums.IconMood), evt.newValue).ToString();
                _node.DialogStepNode.mood = (Enums.IconMood)Enum.Parse(typeof(Enums.IconMood), evt.newValue);
                if (node.DialogStepNode.character)
                {
                    var image = npcIcon.iconImage;
                    image.sprite = node.DialogStepNode.character.GetStyleIcon(_node.DialogStepNode.mood);
                    npcIcon.iconImage = image;
                }
            });

            // Ставим иконку чисто по приколу
            if (node.DialogStepNode.character)
            {
                var image = npcIcon.iconImage;
                image.sprite = node.DialogStepNode.character.GetStyleIcon(_node.DialogStepNode.mood);
                npcIcon.iconImage = image;
            }

            TextField dialogueFieldRu = this.Query<TextField>("Dialogue_Field_Ru");
            dialogueFieldRu.multiline = true;
            dialogueFieldRu.value = node.DialogStepNode.dialogTextRu;
            dialogueFieldRu.RegisterValueChangedCallback(e => { node.DialogStepNode.dialogTextRu = dialogueFieldRu.value; }
            );

            TextField dialogueFieldEn = this.Query<TextField>("Dialogue_Field_En");
            dialogueFieldEn.multiline = true;
            dialogueFieldEn.value = node.DialogStepNode.dialogTextEn;
            dialogueFieldEn.RegisterValueChangedCallback(e => { node.DialogStepNode.dialogTextEn = dialogueFieldEn.value; }
            );

            ObjectField speechField = this.Query<ObjectField>("Speech");
            speechField.value = _node.DialogStepNode.stepSpeech;
            speechField.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.stepSpeech = (AudioClip)speechField.value;
            });

            ObjectField bigPictureField = this.Query<ObjectField>("Big_Picture");
            bigPictureField.value = _node.DialogStepNode.bigPicture;
            bigPictureField.RegisterValueChangedCallback(evt =>
            {
                node.DialogStepNode.bigPicture = (Sprite)bigPictureField.value;
            });

            TextField delayAfterNextTextField = this.Query<TextField>("Delay_After_Step");
            delayAfterNextTextField.value = _node.DialogStepNode.delayAfterNext.ToString();

            delayAfterNextTextField.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.delayAfterNext = float.Parse(delayAfterNextTextField.value, CultureInfo.InvariantCulture);
            });

            ObjectField fastChangesTextField = this.Query<ObjectField>("Fast_Changes");
            fastChangesTextField.value = _node.DialogStepNode.fastChanges;

            fastChangesTextField.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.fastChanges = (FastChangesController)fastChangesTextField.value;
            });

            ObjectField changeRelationNpc = this.Query<ObjectField>("Npc_Relation");
            changeRelationNpc.value = (Npc)_node.DialogStepNode.npcChangeRelation;
            changeRelationNpc.RegisterValueChangedCallback(e =>
                {
                    node.DialogStepNode.npcChangeRelation = (Npc)changeRelationNpc.value;
                }
            );

            TextField changeRelationValue = this.Query<TextField>("Npc_Change_Relation");
            changeRelationValue.value = _node.DialogStepNode.changeRelation.ToString();

            changeRelationValue.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.changeRelation = float.Parse(changeRelationValue.value);
            });

            ObjectField changeMoveToPlayerNpc = this.Query<ObjectField>("Npc_Move_Player");
            changeMoveToPlayerNpc.value = (Npc)_node.DialogStepNode.npcChangeMoveToPlayer;
            changeMoveToPlayerNpc.RegisterValueChangedCallback(e =>
                {
                    node.DialogStepNode.npcChangeMoveToPlayer = (Npc)changeMoveToPlayerNpc.value;
                }
            );

            Toggle moveToPlayerToggle = this.Query<Toggle>("Npc_Move_Player_Toggle");
            moveToPlayerToggle.value = _node.DialogStepNode.newStateMoveToPlayer;
            moveToPlayerToggle.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.newStateMoveToPlayer = moveToPlayerToggle.value;
            });

            TextField karmaValue = this.Query<TextField>("Change_Karma");
            karmaValue.value = _node.DialogStepNode.changeKarma.ToString();

            karmaValue.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.changeKarma = float.Parse(karmaValue.value, CultureInfo.InvariantCulture);
            });

            TextField sanityValue = this.Query<TextField>("Change_Sanity");
            sanityValue.value = _node.DialogStepNode.changeSanity.ToString();

            sanityValue.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.changeSanity = float.Parse(sanityValue.value, CultureInfo.InvariantCulture);
            });
        }
    }
}
#endif