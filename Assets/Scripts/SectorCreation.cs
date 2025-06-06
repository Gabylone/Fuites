using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SectorCreation : Displayable {

    public static SectorCreation Instance;

    public Sector _sector;

    public Image image;
    public Sprite unassigned_Sprite;
    public Sprite assigned_Sprite;

    public bool canSave = false;
    public CanvasGroup saveButton;

    public TMP_InputField name_InputField;

    List<MailButton> buttons = new List<MailButton>();
    public MailButton prefab;
    public Transform parent;

    public RectTransform addMailButton;

    public bool modify = false;

    public RectTransform test;

    private void Awake() {
         Instance = this;
    }

    public void Display(Sector sector) {
        FadeIn();
        
        _sector = sector;
        Sector.current = _sector;

        UpdateDisplay();
    }

    public void OnEditName() {
        _sector.name = name_InputField.text;
        UpdateDisplay();
    }

    public void OnValueChange() {
        
    }

    public void UpdateDisplay() {

        name_InputField.text = _sector.name;

        if (string.IsNullOrEmpty(_sector.name))
            image.sprite = unassigned_Sprite;
        else
            image.sprite = assigned_Sprite;

        foreach (var item in buttons)
            item.Hide();
        for (int i = 0; i < _sector.mailInfos.Count; i++) {
            if (buttons.Count <= i)
                buttons.Add(Instantiate(prefab, parent));
            buttons[i].Display(_sector.mailInfos[i]);
        }

        canSave = true;
        if (string.IsNullOrEmpty(_sector.name)) {
            canSave = false;
        }
        if (_sector.mailInfos.Count == 0) {
            canSave = false;
        } else {
            foreach (var item in _sector.mailInfos) {
                if ( string.IsNullOrEmpty(item.adress) ) {
                    canSave = false;
                }
            }
        }

        saveButton.alpha = canSave ? 1 : .5f;

        addMailButton.SetAsLastSibling();
    }

    public void CreateNewMail() {
        _sector.mailInfos.Add(new MailInfo());
        UpdateDisplay();
    }

    public void ConfirmSector() {
        if (!canSave)
            return;
        Tween.Bounce(saveButton.transform);
        FadeOut();
        if (!modify) {
            Sector.s_sectors.Add(_sector);
            string id = StringUtils.SanitizeFileName(System.DateTime.Now.ToString());
            _sector.id = id;
        }

        _sector.CreateDirectories();
        Sector.current.Save();
        SectorSelection.Instance.Display();
    }

    public void DeleteSector() {
        DisplayMessage.instance.Display($"Voulez-vous vraiment supprimer le secteur {_sector.name} ?", ConfirmDeleteSector);
    }

    public void ConfirmDeleteSector() {
        var sector = Sector.s_sectors.Find(x => x.id == _sector.id);
        if (sector == null) {
            Debug.LogError($"no sector with : {_sector.id} ({_sector.name})");
            return;
        }
        Sector.s_sectors.Remove(sector);
        Directory.Delete(sector.GetPath(), true);
        Debug.Log($"removed sector : {_sector.name}");
        FadeOut();
        SectorSelection.Instance.Display();
    }

    public void RemoveMail(MailInfo mailInfo) {
        var mail = _sector.mailInfos.Find(x => x == mailInfo);
        if (mail == null) {
            Debug.LogError($"No Mail : {mail}");
            return;
        }

        _sector.mailInfos.Remove(mail);
        UpdateDisplay();
    }

    public Sector GetSector() {
    return _sector; }
}
