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
    public NodeData nodeData;
    public NodeViewer visuals;

    public BaseNode(NodeData _data, bool _startNode)
    {
        nodeData = _data != null ? _data : new NodeData();
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
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/VNCreator/Editor/Graph/Node/BaseNodeTemplate.uxml");
        tree.CloneTree(this);

        styleSheets.Add(
            AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/VNCreator/Editor/Graph/Node/BaseNodeStyle.uss"));

        if (_startNode)
        {
            DropdownField dialogStyleDropdownMenu = this.Query<DropdownField>("Style_Dialogs");
            dialogStyleDropdownMenu.choices = Enum.GetNames(typeof(Enums.DialogStyle)).ToList();
            dialogStyleDropdownMenu.value = _node.nodeData.styleOfDialog.ToString();

            // Добавляем обработчик события изменения значения
            dialogStyleDropdownMenu.RegisterValueChangedCallback(evt =>
            {
                dialogStyleDropdownMenu.value = Enum.Parse(typeof(Enums.DialogStyle), evt.newValue).ToString();
                _node.nodeData.styleOfDialog =
                    (Enums.DialogStyle)Enum.Parse(typeof(Enums.DialogStyle), evt.newValue);
            });

            Toggle moreReadToggle = this.Query<Toggle>("More_Read");
            moreReadToggle.value = _node.nodeData.moreRead;
            moreReadToggle.RegisterValueChangedCallback(evt => { _node.nodeData.moreRead = moreReadToggle.value; });

            Toggle canMoveToggle = this.Query<Toggle>("Can_Move");
            canMoveToggle.value = _node.nodeData.canMove;
            canMoveToggle.RegisterValueChangedCallback(evt => { _node.nodeData.canMove = canMoveToggle.value; });

            Toggle canInterToggle = this.Query<Toggle>("Can_Inter");
            canInterToggle.value = _node.nodeData.canInter;
            canInterToggle.RegisterValueChangedCallback(evt => { _node.nodeData.canInter = canInterToggle.value; });

            Toggle darkAfterEndToggle = this.Query<Toggle>("Dark_End");
            darkAfterEndToggle.value = _node.nodeData.darkAfterEnd;
            darkAfterEndToggle.RegisterValueChangedCallback(evt =>
            {
                _node.nodeData.darkAfterEnd = darkAfterEndToggle.value;
            });

            TextField mainPanelDelayTextField = this.Query<TextField>("MainPanelStartDelay");
            mainPanelDelayTextField.value = _node.nodeData.mainPanelStartDelay.ToString();

            mainPanelDelayTextField.RegisterValueChangedCallback(evt =>
            {
                _node.nodeData.mainPanelStartDelay = float.Parse(mainPanelDelayTextField.value);
            });
        }
        else
        {
            Foldout dialogSettingFoldout = this.Query<Foldout>("Dialog_Settings");
            dialogSettingFoldout.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        TextField charNameField = this.Query<TextField>("Character");
        charNameField.value = _node.nodeData.characterName;
        charNameField.RegisterValueChangedCallback(e => { node.nodeData.characterName = charNameField.value; }
        );

        DropdownField moodDropdownMenu = this.Query<DropdownField>("Char_Mood");
        moodDropdownMenu.choices = Enum.GetNames(typeof(Enums.IconMood)).ToList();
        moodDropdownMenu.value = _node.nodeData.mood.ToString();

        moodDropdownMenu.RegisterValueChangedCallback(evt =>
        {
            moodDropdownMenu.value = Enum.Parse(typeof(Enums.IconMood), evt.newValue).ToString();
            _node.nodeData.mood = (Enums.IconMood)Enum.Parse(typeof(Enums.IconMood), evt.newValue);
        });

        TextField dialogueFieldRu = this.Query<TextField>("Dialogue_Field_Ru");
        dialogueFieldRu.multiline = true;
        dialogueFieldRu.value = node.nodeData.dialogTextRu;
        dialogueFieldRu.RegisterValueChangedCallback(e => { node.nodeData.dialogTextRu = dialogueFieldRu.value; }
        );

        TextField dialogueFieldEn = this.Query<TextField>("Dialogue_Field_En");
        dialogueFieldEn.multiline = true;
        dialogueFieldEn.value = node.nodeData.dialogTextEn;
        dialogueFieldEn.RegisterValueChangedCallback(e => { node.nodeData.dialogTextEn = dialogueFieldEn.value; }
        );

        Toggle cursedTextToggle = this.Query<Toggle>("Cursed_Text");
        cursedTextToggle.value = _node.nodeData.cursedText;
        cursedTextToggle.RegisterValueChangedCallback(evt => { _node.nodeData.cursedText = cursedTextToggle.value; });

        ObjectField speechField = this.Query<ObjectField>("Speech");
        speechField.value = _node.nodeData.stepSpeech;
        speechField.RegisterValueChangedCallback(evt => { _node.nodeData.stepSpeech = (AudioClip)speechField.value; });

        ObjectField bigPictureField = this.Query<ObjectField>("Big_Picture");
        if (_node.nodeData.styleOfDialog == Enums.DialogStyle.BigPicture)
        {
            bigPictureField.value = _node.nodeData.bigPicture;
            bigPictureField.RegisterValueChangedCallback(evt =>
            {
                _node.nodeData.bigPicture = (Sprite)bigPictureField.value;
            });
        }
        else
            bigPictureField.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        TextField activateCutsceneStepTextField = this.Query<TextField>("Activate_Cutscene_Step");
        activateCutsceneStepTextField.value = _node.nodeData.activateCutsceneStep.ToString();

        activateCutsceneStepTextField.RegisterValueChangedCallback(evt =>
        {
            _node.nodeData.activateCutsceneStep = int.Parse(activateCutsceneStepTextField.value);
        });

        TextField delayAfterNextTextField = this.Query<TextField>("Delay_After_Step");
        delayAfterNextTextField.value = _node.nodeData.delayAfterNext.ToString();

        delayAfterNextTextField.RegisterValueChangedCallback(evt =>
        {
            _node.nodeData.delayAfterNext = float.Parse(delayAfterNextTextField.value);
        });

        TextField fastChangesTextField = this.Query<TextField>("Fast_Changes");
        fastChangesTextField.value = _node.nodeData.fastChangesName;

        fastChangesTextField.RegisterValueChangedCallback(evt =>
        {
            _node.nodeData.fastChangesName = fastChangesTextField.value;
        });
    }
}
#endif