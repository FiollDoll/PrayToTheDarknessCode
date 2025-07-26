using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
                _node.DialogStepNode.mainPanelStartDelay = float.Parse(mainPanelDelayTextField.value);
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

        Toggle cursedTextToggle = this.Query<Toggle>("Cursed_Text");
        cursedTextToggle.value = _node.DialogStepNode.cursedText;
        cursedTextToggle.RegisterValueChangedCallback(evt =>
        {
            _node.DialogStepNode.cursedText = cursedTextToggle.value;
        });

        ObjectField speechField = this.Query<ObjectField>("Speech");
        speechField.value = _node.DialogStepNode.stepSpeech;
        speechField.RegisterValueChangedCallback(evt =>
        {
            _node.DialogStepNode.stepSpeech = (AudioClip)speechField.value;
        });

        ObjectField bigPictureField = this.Query<ObjectField>("Big_Picture");
        if (_node.DialogStepNode.styleOfDialog == Enums.DialogStyle.BigPicture)
        {
            bigPictureField.value = _node.DialogStepNode.bigPicture;
            bigPictureField.RegisterValueChangedCallback(evt =>
            {
                _node.DialogStepNode.bigPicture = (Sprite)bigPictureField.value;
            });
        }
        else
            bigPictureField.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        TextField activateCutsceneStepTextField = this.Query<TextField>("Activate_Cutscene_Step");
        activateCutsceneStepTextField.value = _node.DialogStepNode.activateCutsceneStep.ToString();

        activateCutsceneStepTextField.RegisterValueChangedCallback(evt =>
        {
            _node.DialogStepNode.activateCutsceneStep = int.Parse(activateCutsceneStepTextField.value);
        });

        TextField delayAfterNextTextField = this.Query<TextField>("Delay_After_Step");
        delayAfterNextTextField.value = _node.DialogStepNode.delayAfterNext.ToString();

        delayAfterNextTextField.RegisterValueChangedCallback(evt =>
        {
            _node.DialogStepNode.delayAfterNext = float.Parse(delayAfterNextTextField.value);
        });

        ObjectField fastChangesTextField = this.Query<ObjectField>("Fast_Changes");
        fastChangesTextField.value = _node.DialogStepNode.fastChanges;

        fastChangesTextField.RegisterValueChangedCallback(evt =>
        {
            _node.DialogStepNode.fastChanges = (FastChangesController)fastChangesTextField.value;
        });
    }
}
#endif