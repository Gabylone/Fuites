using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using iTextSharp.text.pdf;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using iTextSharp.text;
using NUnit.Framework.Internal;
using Font = iTextSharp.text.Font;

public class PDFExport : MonoBehaviour {

    public static PDFExport Instance;

    iTextSharp.text.Document pdfdoc;
    iTextSharp.text.Image pdfImg;  
	iTextSharp.text.pdf.PdfWriter pdfwriter; 

	iTextSharp.text.Rectangle currentPageSize = new iTextSharp.text.Rectangle (1654,2339);//iTextSharp.text.PageSize.A0;

	int gridSideLength = 20;// side length of each small square grid
	int gridCountInBlockRowHorizontal;// = currentPageSize.Width / gridSideLength;

    public static float frameWidth = 800f;
    public static float frameHeight = 300f;
    public static float spacing = 20f;
    /// INIT FONTS ///
    string fontPath;
    string fontBoldPath;
    string fontLightPath;
    BaseFont font_medium;
    BaseFont font_bold;
    BaseFont font_light;

    // set up font sizes
    public int normal_FontSize = 29;
    public int adress_FontSize = 25;
    public int contact_FontSize = 33;
    public int smallTitle_FontSize = 55;
    public int bigTitle_FontSize = 65;
    public float margin_X = 200f;
    public float margin_Y = 730f;

    private void Awake() {
        Instance = this;    
    }

    private void Start() {

        /// INIT FONTS ///
        /*fontPath = $"{Application.streamingAssetsPath}/Fonts/Poppins-Medium.ttf";
        fontBoldPath = $"{Application.streamingAssetsPath}/Fonts/Poppins-Bold.ttf";
        fontLightPath = $"{Application.streamingAssetsPath}/Fonts/Poppins-Light.ttf";*/
        StartCoroutine(LoadFont("Poppins-Medium", 0));
        StartCoroutine(LoadFont("Poppins-Bold", 1));
        StartCoroutine(LoadFont("Poppins-Light", 2));
        StartCoroutine(LoadLogo());
        /*font_medium = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        font_bold  = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        font_light = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);*/

        /*string path1 = $"{Application.persistentDataPath}/pdf_bg_1.png";
        if (!File.Exists(path1)) {
            var tex = Resources.Load("pdf_bg_1") as Texture2D;
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path1, bytes);
            return;
        }

        string path2 = $"{Application.persistentDataPath}/pdf_bg_2.png";
        if (!File.Exists(path2)) {
            var tex = Resources.Load("pdf_bg_2") as Texture2D;
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path2, bytes);
            return;
        }*/

        //Test();

    }

    public void Test() {
        // TEST //
        var testSector = new Sector();
        testSector.location = "Marseille";
        testSector.name = "test_sector";
        for (int i = 0; i < 2; i++) {
            var newLeak = new Leak();
            newLeak.id = $"test_{i+1}";
            newLeak.adress = $"3{i} rue de la Mouette";
            newLeak.date = "30 juin 1995";
            newLeak.name = $"Fuite N°{i+1}";
            newLeak.leakType = UnityEngine.Random.Range(0, 5);
            newLeak.urgency = UnityEngine.Random.Range(0, 3);
            newLeak.material = "Brique ?";
            newLeak.infos = "Informations complémentaires";
            testSector.leaks.Add(newLeak);
        }
        Sector.current = testSector;
        GenerateReport_Sector(testSector);
    }

    public string GenerateReport_Sector(Sector sector) {
		string destinationPDFPath = $"{sector.GetPath()}/secteur_{sector.id}.pdf";
		CreateFromRawdataFile (destinationPDFPath, sector);
        return destinationPDFPath;
	}

    public string GenerateReport_Leak(Leak leak) {
        string destinationPDFPath = $"{leak.GetPath()}/fuite_{leak.id}.pdf";
        PrintOnlyLeak(destinationPDFPath, leak);
        return destinationPDFPath;
    }

    void InitPDF(string path) {
        // init pdf doc.
        pdfdoc = new iTextSharp.text.Document();
        pdfdoc.SetPageSize(currentPageSize);
        gridCountInBlockRowHorizontal = (int)(currentPageSize.Width / gridSideLength);
        if (File.Exists(path))
            File.Delete(path);
        pdfwriter = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfdoc, new FileStream(path, FileMode.CreateNew));
        pdfdoc.Open();
        pdfdoc.NewPage();
    }
    void CreateFromRawdataFile(string destinationPDFPath, Sector sector) {

        InitPDF(destinationPDFPath);
        iTextSharp.text.pdf.PdfContentByte cb = pdfwriter.DirectContent;


        // init colors
        Color c;
        ColorUtility.TryParseHtmlString("#221F21", out c);
        BaseColor pdf_Black = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#3495D1", out c);
        BaseColor pdf_Blue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#2D67A5", out c);
        BaseColor pdf_DarkBlue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));

        // PLACE LOGO //
        string logoPath = $"{Application.persistentDataPath}/logo.png";
        pdfImg = iTextSharp.text.Image.GetInstance(logoPath);
        pdfImg.SetAbsolutePosition(240, currentPageSize.Height - 300);
        pdfImg.ScaleAbsolute(160, 220);
        pdfdoc.Add(pdfImg);

        cb.SetFontAndSize(font_bold, 28);
        cb.BeginText();
        cb.SetTextMatrix(200, currentPageSize.Height - 340);
        cb.SetColorFill(pdf_Blue);
        cb.ShowText("AQUA");
        cb.SetTextMatrix(288, currentPageSize.Height - 340);
        cb.SetColorFill(pdf_DarkBlue);
        cb.ShowText("RESSOURCE");
        cb.EndText();

        // ADRESS
        cb.SetFontAndSize(font_light, adress_FontSize);
        cb.BeginText();
        cb.SetColorFill(pdf_Black);
        cb.SetTextMatrix(487, currentPageSize.Height - 170);
        cb.ShowText("SARL au capital de 5000 euros");
        cb.SetTextMatrix(487, currentPageSize.Height - 200);
        cb.ShowText("RCS : ROUEN 903 207 876 - TVA Intracom : FR76 903 207 876");
        cb.SetTextMatrix(487, currentPageSize.Height - 230);
        cb.ShowText("6 rue de la Charmille 76 770 Le Houlme");
        cb.EndText();

        // CONTACT
        cb.SetFontAndSize(font_bold, contact_FontSize);
        cb.BeginText();
        cb.SetTextMatrix(487, currentPageSize.Height - 300);
        cb.ShowText("contact@aquaressource.com");
        cb.SetTextMatrix(487, currentPageSize.Height - 340);
        cb.ShowText("Tél : 07.80.98.23.67");
        cb.EndText();

        // RECTANGLE SEPARATION
        cb.Rectangle(200, currentPageSize.Height -440, 800, 15);
        cb.Fill();

        // BIG TITLE
        cb.SetFontAndSize(font_bold, bigTitle_FontSize);
        cb.BeginText();
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 520);
        cb.ShowText("compte rendu d'intervention".ToUpper());
        cb.EndText();

        // SMALL TITLE
        cb.SetFontAndSize(font_bold, smallTitle_FontSize);
        cb.BeginText();
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 590);
        cb.ShowText("CAMPAGNE N°6".ToUpper());
        cb.EndText();


        // BLUE TITLES //
        cb.SetFontAndSize(font_bold, normal_FontSize);
        cb.BeginText();
        cb.SetColorFill(pdf_Blue);
        // date d'intervention
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 730);
        cb.ShowText("date d'intervention".ToUpper());
        // lieu
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 860);
        cb.ShowText("lieu".ToUpper());
        // investigations réalisées
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 980);
        cb.ShowText("investigations réalisées".ToUpper());
        // resultats
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 1150);
        cb.ShowText("résultats".ToUpper());
        cb.EndText();

        // BLUE INFOS //
        cb.SetFontAndSize(font_light, normal_FontSize);
        cb.BeginText();
        cb.SetColorFill(pdf_Black);
        // date d'intervention
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 770);
        if (sector.leaks.Count < 1)
            cb.ShowText($"pas d'interventions");
        else if (sector.leaks.Count == 1)
            cb.ShowText($"le {sector.leaks.First().date}");
        else
            cb.ShowText($"du {sector.leaks.First().date} au {sector.leaks.Last().date}");

        // lieu
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 900);
        cb.ShowText(sector.location);
        // investigations réalisées
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 1020);
        cb.ShowText("Contrôle du réseau par écoutes systématiques, localisation des fuites par");
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 1060);
        cb.ShowText("corrélations acoustiques et écoutes au sol");

        // resultats
        cb.SetTextMatrix(margin_X, currentPageSize.Height - 1190);
        cb.ShowText("Les investigations réalisées ont permis de localiser");
        cb.SetFontAndSize(font_bold, normal_FontSize);
        cb.ShowText(sector.leaks.Count > 1 ? $" {sector.leaks.Count} fuites " : $" {sector.leaks.Count} fuite ");
        cb.SetFontAndSize(font_medium, normal_FontSize);
        cb.ShowText("au total :");

        int decal = 1;
        cb.SetFontAndSize(font_bold, normal_FontSize);
        for (int leakType = 0; leakType < 6; leakType++) {
            var leaks = sector.leaks.FindAll(x => x.leakType == leakType);
            if (leaks.Count == 0)
                continue;
            string word = leaks.Count > 1 ? "fuites" : "fuite";
            string phrase = $"- {leaks.Count} {word} {Leak.leaksTypes_text[leakType]}";
            cb.SetTextMatrix(margin_X, currentPageSize.Height - (1190+(decal*40)));
            cb.ShowText(phrase);
            ++decal;
        }
        cb.EndText();

        // SECTOR MAP
        // afficher "prendre photo du secteur dans la page de rapport" quand on a pas fait de trucs ?
        /*float w = currentPageSize.Width - (margin_X*2);
        float h = w * 9 / 16;
        cb.Rectangle(margin_X, margin_X, w, h);
        cb.Fill();*/
        DisplayCrop($"{Sector.current.GetPath()}", "sector_map", margin_X, 150);


        // 2870
        for (int i = 0; i < sector.leaks.Count; i++) {
            pdfdoc.NewPage();
            PrintLeak(sector.leaks[i], cb);
        }

        pdfdoc.Dispose();
        DIsplayLoading.instance.FadeOut();
        if (Application.isEditor)
            System.Diagnostics.Process.Start(destinationPDFPath);
	}

    IEnumerator LoadLogo() {
        string sourcePath = $"{Application.streamingAssetsPath}/logo.png";
        string targetPath = $"{Application.persistentDataPath}/logo.png";

        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(sourcePath);
        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success) {
            System.IO.File.WriteAllBytes(targetPath, www.downloadHandler.data);
            Debug.Log("bien copiée");
        } else {
            Debug.LogError($"Erreur lors de la copie de la font : {www.error}");
            yield break;
        }
    }

    IEnumerator LoadFont(string fontName, int i) {
        string sourcePath = $"{Application.streamingAssetsPath}/Fonts/{fontName}.ttf";
        string targetPath = $"{Application.persistentDataPath}/{fontName}.ttf";

        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(sourcePath);
        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success) {
            System.IO.File.WriteAllBytes(targetPath, www.downloadHandler.data);
            Debug.Log("bien copiée");
        } else {
            Debug.LogError($"Erreur lors de la copie de la font : {www.error}");
            yield break;
        }
        // Maintenant, targetPath est un vrai chemin local
        switch (i) {
            case 0:
                font_medium = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                break;
            case 1:
                font_bold = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                break;
            case 2:
                font_light = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                break;
            default:

                break;
        }

        if (font_medium == null) {
            Debug.Log($"Font non chargée !");
        }
    }

    void PrintOnlyLeak(string destinationPDFPath, Leak leak) {

        InitPDF(destinationPDFPath);
        iTextSharp.text.pdf.PdfContentByte cb = pdfwriter.DirectContent;

        // init colors
        Color c;
        ColorUtility.TryParseHtmlString("#221F21", out c);
        BaseColor pdf_Black = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#3495D1", out c);
        BaseColor pdf_Blue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#2D67A5", out c);
        BaseColor pdf_DarkBlue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));

        PrintLeak(leak, cb);

        pdfdoc.Dispose();
        DIsplayLoading.instance.FadeOut();
        if (Application.isEditor)
            System.Diagnostics.Process.Start(destinationPDFPath);
	}

    void PrintLeak(Leak leak, iTextSharp.text.pdf.PdfContentByte cb) {

        /// LEAKS ///
        int marge_Title = 230;
        int marge_Info = 640;
        int step = 60;
        int leakHeight = 450;

        // init colors
        Color c;
        ColorUtility.TryParseHtmlString("#221F21", out c);
        BaseColor pdf_Black = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#3495D1", out c);
        BaseColor pdf_Blue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));
        ColorUtility.TryParseHtmlString("#2D67A5", out c);
        BaseColor pdf_DarkBlue = new BaseColor(Mathf.RoundToInt(c.r * 255), Mathf.RoundToInt(c.g * 255), Mathf.RoundToInt(c.b * 255));

        // LOGO //
        string logoPath = $"{Application.persistentDataPath}/logo.png";
        pdfImg = iTextSharp.text.Image.GetInstance(logoPath);
        pdfImg.SetAbsolutePosition(1320, currentPageSize.Height - 280);
        pdfImg.ScaleAbsolute(160, 220);
        pdfdoc.Add(pdfImg);
        cb.SetFontAndSize(font_bold, 28);
        cb.BeginText();
        cb.SetTextMatrix(1275, currentPageSize.Height - 325);
        cb.SetColorFill(pdf_Blue);
        cb.ShowText("AQUA");
        cb.SetTextMatrix(1360, currentPageSize.Height - 325);
        cb.SetColorFill(pdf_DarkBlue);
        cb.ShowText("RESSOURCE");
        cb.EndText();

        // TITLE //
        cb.SetFontAndSize(font_bold, 75);
        cb.BeginText();
        cb.SetColorFill(pdf_Black);
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - 320);
        cb.ShowText(leak.name.ToUpper());
        cb.EndText();

        // RECTANGLE SEPARATION
        cb.Rectangle(600, currentPageSize.Height -320, 630, 15);
        cb.Fill();
        switch (leak.urgency) {
            case 0:
                cb.SetColorFill(BaseColor.GREEN);
                break;
            case 1:
                cb.SetColorFill(BaseColor.YELLOW);
                break;
            case 2:
                cb.SetColorFill(BaseColor.RED);
                break;
            default:
                break;
        }
        cb.Rectangle(marge_Info, currentPageSize.Height - (leakHeight + 5 + (step*4)), 100, 40);
        cb.Fill();

        // BLUE TITLES
        cb.SetFontAndSize(font_bold, normal_FontSize);
        cb.BeginText();
        cb.SetColorFill(pdf_Blue);
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*0)));
        cb.ShowText("ADRESSE");
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*1)));
        cb.ShowText($"COORDONNEES GPS");
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*2)));
        cb.ShowText($"NATURE DE LA FUITE");
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*3)));
        cb.ShowText($"MATERIAU");
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*4)));
        cb.ShowText($"URGENCE DE LA FUITE");
        cb.SetTextMatrix(marge_Title, currentPageSize.Height - (leakHeight + (step*5)));
        cb.ShowText($"INFORMATIONS");
        cb.EndText();

        // INFOS
        cb.SetFontAndSize(font_medium, normal_FontSize);
        cb.BeginText();
        cb.SetColorFill(pdf_Black);
        cb.SetTextMatrix(marge_Info, currentPageSize.Height - (leakHeight + (step*0)));
        cb.ShowText(leak.adress);
        cb.SetTextMatrix(marge_Info, currentPageSize.Height - (leakHeight + (step*1)));
        cb.ShowText($"{leak.latitude}, {leak.longitude}");
        cb.SetTextMatrix(marge_Info, currentPageSize.Height - (leakHeight + (step*2)));
        cb.ShowText($"{leak.GetLeakTypeText()}");
        cb.SetTextMatrix(marge_Info, currentPageSize.Height - (leakHeight + (step*3)));
        cb.ShowText($"{leak.material}");
        cb.SetTextMatrix(marge_Info + 120, currentPageSize.Height - (leakHeight + (step*4)));
        cb.ShowText($"{leak.GetUrgency_text()}");
        cb.SetTextMatrix(marge_Info, currentPageSize.Height - (leakHeight + (step*5)));
        cb.ShowText($"{leak.infos}");
        cb.EndText();

        DisplayCrop($"{Sector.current.GetPath()}/{leak.GetFolderName()}", "map", margin_X, 820);

        var originalSizes = new List<Vector2>();
        var paths = new List<string>();
        for (int j = 0; j < 3; j++) {
            string photoPath = $"{Sector.current.GetPath()}/{leak.GetFolderName()}/photo_{j+1}.png";
            if (File.Exists(photoPath)) {
                var rawData = File.ReadAllBytes(photoPath);
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(rawData);
                originalSizes.Add(new Vector2(tex.width, tex.height));
                paths.Add(photoPath);
            }
        }
        if (originalSizes.Count > 0) {
            frameWidth = currentPageSize.Width - (marge_Title*2);
            frameHeight = 600;
            var rects = CalculateImageRects(originalSizes);
            for (int photoIndex = 0; photoIndex < paths.Count; photoIndex++) {
                pdfImg = iTextSharp.text.Image.GetInstance(paths[photoIndex]);
                var rect = rects[photoIndex];
                pdfImg.SetAbsolutePosition(rect.x, 150);
                pdfImg.ScaleAbsolute(rect.width, rect.height);
                pdfdoc.Add(pdfImg);
            }
        }
        
    }

    void DisplayCrop(string path, string fileName, float x, float y) {
        string target = $"{path}/{fileName}.png";
        if (File.Exists(target)) {
            string croppedMap_path = $"{path}/{fileName}_cropped.png";
            // cropping sector map
            if (!File.Exists(croppedMap_path)) {
                var rawData = File.ReadAllBytes(target);
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(rawData);
                tex = CropTo16by9(tex);
                var bytes = tex.EncodeToPNG();
                File.WriteAllBytes(croppedMap_path, bytes);
            }

            pdfImg = iTextSharp.text.Image.GetInstance(croppedMap_path);
            pdfImg.SetAbsolutePosition(x, y);
            Debug.Log($"image scale : {pdfImg.Width}/{pdfImg.Height}");
            var map_width = 0f;
            var map_height = 0f;
            map_width = currentPageSize.Width - (x*2);
            map_height = map_width * pdfImg.Height / pdfImg.Width;

            pdfImg.ScaleAbsolute(map_width, map_height);
            pdfdoc.Add(pdfImg);
        } else {
            Debug.Log($"no map");
        }
    }

    public static Texture2D CropTo16by9(Texture2D originalTexture) {
        int width = originalTexture.width;

        // Calcul de la hauteur cible en format 16:9
        int targetHeight = Mathf.RoundToInt(width * 9f / 16f);

        // Si la texture est déjà en 16:9 ou plus large, on ne fait rien
        if (originalTexture.height <= targetHeight) {
            Debug.LogError("La hauteur de l'image est déjà trop petite pour un crop en 16:9. Aucun rognage effectué.");
            return originalTexture;
        }

        // Calcul du décalage vertical pour centrer le crop
        int yOffset = (originalTexture.height - targetHeight) / 2;

        // Récupération des pixels de la zone à conserver
        Color[] croppedPixels = originalTexture.GetPixels(0, yOffset, width, targetHeight);

        // Création de la nouvelle texture
        Texture2D croppedTexture = new Texture2D(width, targetHeight, originalTexture.format, false);
        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        return croppedTexture;
    }


    public void OpenPDFAndroid(string filePath) {
    try
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW");
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject file = new AndroidJavaObject("java.io.File", filePath);
        AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromFile", file);

        intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/pdf");
        intent.Call<AndroidJavaObject>("addFlags", 1 << 0x00000020); // FLAG_GRANT_READ_URI_PERMISSION

        currentActivity.Call("startActivity", intent);
    }
    catch (Exception e)
    {
        Debug.LogError("Erreur lors de l'ouverture du PDF : " + e.Message);
    }
    }


    public struct ImageRect {
        public float x, y, width, height;
    }

    public List<ImageRect> CalculateImageRects(List<Vector2> originalSizes) {


        int imageCount = originalSizes.Count;
        if (imageCount == 0)
            return new List<ImageRect>();

        // Étape 1 : redimensionner chaque image pour que sa hauteur = frameHeight
        List<Vector2> scaledToHeight = new List<Vector2>();
        foreach (var size in originalSizes) {
            float scale = frameHeight / size.y;
            Vector2 scaled = size * scale;
            scaledToHeight.Add(scaled);
        }

        // Étape 2 : calculer la largeur totale avec espacements
        float totalWidth = spacing * (imageCount - 1);
        foreach (var s in scaledToHeight)
            totalWidth += s.x;

        float scaleFactor = 1f;
        if (totalWidth > frameWidth) {
            // Trop large, on réduit tout proportionnellement
            scaleFactor = frameWidth / totalWidth;
        }

        // Étape 3 : appliquer le scale final
        List<Vector2> finalSizes = new List<Vector2>();
        foreach (var size in scaledToHeight)
            finalSizes.Add(size * scaleFactor);

        float finalTotalWidth = spacing * (imageCount - 1);
        foreach (var s in finalSizes)
            finalTotalWidth += s.x;

        // Étape 4 : centrer dans la page
        float startX = (currentPageSize.Width - finalTotalWidth) / 2f;
        float y = 0f; // en bas de la page

        List<ImageRect> result = new List<ImageRect>();
        float currentX = startX;

        foreach (var size in finalSizes) {
            result.Add(new ImageRect {
                x = currentX,
                y = y,
                width = size.x,
                height = size.y
            });
            currentX += size.x + spacing;
        }

        return result;


    }


}
