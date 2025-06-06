using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Mail;
using TMPro;
using System.IO;

public class MailManager : MonoBehaviour
{
    public static MailManager instance;
    private Sector _sector;
    private Leak _leak;

    bool onlyLeak = false;

    private void Awake() {
        instance = this;
    }


    public void SendLeakReport(Leak leak) {
        onlyLeak = true;
        _sector = Sector.current;
        _leak = leak;
        SendEmails();
    }
    public void SendSectorReport() {
        onlyLeak = false;
        _sector = Sector.current;
        SendEmails();
    }
    void SendEmails() {
        Invoke("SendEmailsDelay", 0.1f);
        
    }

    void SendEmailsDelay() {
        string message = $"Voulez vous envoyer le rapport aux adresses suivantes:";
        foreach (var item in _sector.mailInfos)
            message+= $"\n<b><i>{item.adress}</b></i>";

        DisplayMessage.instance.Display(message, HandleOnConfirm);
    }

    void HandleOnConfirm() {
        StartCoroutine(SendEmailsCoroutine());
    }

    IEnumerator SendEmailsCoroutine() {

        Debug.Log($"send emails coroutine");
        DIsplayLoading.instance.Display("Création du rapport");
        yield return new WaitForSeconds(3f);

        Debug.Log("mrrde");
        string pdfPath = "";
        if (onlyLeak) {
            pdfPath = PDFExport.Instance.GenerateReport_Leak(_leak);
        } else {
            pdfPath = PDFExport.Instance.GenerateReport_Sector(_sector);
        }

        Debug.Log(pdfPath);

        yield return new WaitForSeconds(1f);

        DIsplayLoading.instance.UpdateText("Envoi des rapports..."); 
        yield return new WaitForSeconds(1f);

        foreach (var mailInfo in _sector.mailInfos) {
            string message = $"Envoie du rapport:";
            message+= $"\n<b><i>{mailInfo.adress}</b></i>";
            DIsplayLoading.instance.UpdateText(message);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("gabrielsarnikov@gmail.com");
            mail.To.Add(mailInfo.adress);


            if (onlyLeak) {
                mail.Subject = $"Rapport Fuite - {_leak.name}";
                mail.Body = $"Trouvez en pièce jointe le rapport de la fuite {_leak.name}";
            } else {
                mail.Subject = $"Rapport Secteur - {_sector.name}";
                mail.Body = $"Trouvez en pièce jointe le rapport du secteur {_sector.name}";
            }

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(pdfPath);
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("gabrielsarnikov@gmail.com", "lwrm knze wsag yaeh");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            yield return new WaitForSeconds(1f);
            DIsplayLoading.instance.UpdateText("Message envoyé");
            yield return new WaitForSeconds(0.5f);

        }

        DIsplayLoading.instance.UpdateText("Fin du l'envoi des mails");
        DIsplayLoading.instance.FadeOut();
    }

}
