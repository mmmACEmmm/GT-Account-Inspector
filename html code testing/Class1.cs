using UnityEngine;
using BepInEx;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PlayFabSteamAuth
{
    [BepInPlugin("com.yourname.playfabsteamauth", "PlayFab Steam Auth", "1.0.0")]
    public class PlayFabSteamAuthMod : BaseUnityPlugin
    {
        private bool showGUI = true;
        private bool showWellKnownPage = false;
        private string playfabId = "";
        private string steamTicket = "";
        private string accountInfoText = "";
        private Vector2 accountInfoScrollPosition;
        private Vector2 wellKnownScrollPosition;
        private string selectedPlayerId = "";
        private string sessionTicket = "";
        private Dictionary<string, List<PlayerInfo>> groupedPlayers = new Dictionary<string, List<PlayerInfo>>();
        private Dictionary<string, bool> expandedGroups = new Dictionary<string, bool>();

        private const string PlayFabTitleId = "63FDD"; // Your PlayFab Title ID

        private class PlayerInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
        }

        void Start()
        {
            InitializePlayerList();
        }

        void InitializePlayerList()
        {
            List<PlayerInfo> players = new List<PlayerInfo>
            {
                new PlayerInfo { Id = "9DBC90CF7449EF64", FullName = "StyledSnail" },
                new PlayerInfo { Id = "6FE5FF4D5DF68843", FullName = "Pine" },
                new PlayerInfo { Id = "52529F0635BE0CDF", FullName = "PapaSmurf" },
                new PlayerInfo { Id = "10D31D3BDCCE5B1F", FullName = "Deezey" },
                new PlayerInfo { Id = "BAC5807405123060", FullName = "britishmonke" },
                new PlayerInfo { Id = "A6FFC7318E1301AF", FullName = "jmancurl" },
                new PlayerInfo { Id = "3B9FD2EEF24ACB3", FullName = "VMT" },
                new PlayerInfo { Id = "A04005517920EB0", FullName = "K9" },
                new PlayerInfo { Id = "33FFA45DBFD33B01", FullName = "will" },
                new PlayerInfo { Id = "D6971CA01F82A975", FullName = "Elliot" },
                new PlayerInfo { Id = "636D8846E76C9B5A", FullName = "Clown" },
                new PlayerInfo { Id = "65CB0CCF1AED2BF", FullName = "Ethyb" },
                new PlayerInfo { Id = "48437FE432DE48BE", FullName = "BBVR" },
                new PlayerInfo { Id = "5AA1231973BE8A62", FullName = "Apollo" },
                new PlayerInfo { Id = "CBCCBBB6C28A94CF", FullName = "PTMstar" },
                new PlayerInfo { Id = "6DC06EEFFE9DBD39", FullName = "Lucio" },
                new PlayerInfo { Id = "4ACA3C76B334B17F", FullName = "Wihz" },
                new PlayerInfo { Id = "571776944B6162F1", FullName = "CubCub" },
                new PlayerInfo { Id = "645222265FB972B", FullName = "Chaotic Asriel" },
                new PlayerInfo { Id = "EE9FB127CF7DBBD5", FullName = "NOTMARK" },
                new PlayerInfo { Id = "54DCB69545BE0800", FullName = "Biffbish" },
                new PlayerInfo { Id = "3CB4F61C87A5AF24", FullName = "Octoburr/Evelyn" },
                new PlayerInfo { Id = "4994748F8B361E31", FullName = "Octoburr/Evelyn" },
                new PlayerInfo { Id = "5ACE0508B3B95588", FullName = "ACEGT" },
                new PlayerInfo { Id = "1CF4862F9A7B0D39", FullName = "ACEGT" },
                new PlayerInfo { Id = "AAB44BFD0BA34829", FullName = "Boda" },
                new PlayerInfo { Id = "61AD990FF3A423B7", FullName = "Boda" },
                new PlayerInfo { Id = "80279945E7D3B57D", FullName = "Jolyne" },
                new PlayerInfo { Id = "C3878B068886F6C3", FullName = "ZZEN" },
                new PlayerInfo { Id = "F08CE3118F9E793E", FullName = "TurboAlligator" },
                new PlayerInfo { Id = "D6E20BE9655C798", FullName = "TTTPIG 1" },
                new PlayerInfo { Id = "71AA09D13C0F408D", FullName = "TTTPIG 2" },
                new PlayerInfo { Id = "1D6E20BE9655C798", FullName = "TTTPIG 3" },
                new PlayerInfo { Id = "22A7BCEFFD7A0BBA", FullName = "TTTPIG 4" },
                new PlayerInfo { Id = "6F79BE7CB34642AC", FullName = "CodyO'Quinn" },
                new PlayerInfo { Id = "CBCCBBB6C28A94CF", FullName = "PTMstar" },
                new PlayerInfo { Id = "7E44E8337DF02CC1", FullName = "Nunya" },
                new PlayerInfo { Id = "42C809327652ECDD", FullName = "ElectronicWall 2" },
                new PlayerInfo { Id = "7F31BEEC604AE189", FullName = "ElectronicWall 1" },
                new PlayerInfo { Id = "ECDE8A2FF8510934", FullName = "Antoca" },
                new PlayerInfo { Id = "DE601BC40DB68CE0", FullName = "Graic" },
                new PlayerInfo { Id = "F5B5C64914C13B83", FullName = "HatGirl" },
                new PlayerInfo { Id = "498D4C2F23853B37", FullName = "POGTROLL" },
                new PlayerInfo { Id = "D0CB396539676DD8", FullName = "FrogIlla" },
                new PlayerInfo { Id = "A1A99D33645E4A94", FullName = "STEAMVRAVTS / YEAT" },
                new PlayerInfo { Id = "CA8FDFF42B7A1836", FullName = "Brokenstone" },
                new PlayerInfo { Id = "5CCCAA8A225A468B", FullName = "furina" },
                new PlayerInfo { Id = "ABD60175B46E45C5", FullName = "SALTWATER" },
                new PlayerInfo { Id = "6713DA80D2E9BFB5", FullName = "AHauntedArmy" },
                new PlayerInfo { Id = "B4A3FF01312B55B1", FullName = "Pluto" },
                new PlayerInfo { Id = "E354E818871BD1D8", FullName = "developer9998" },
                new PlayerInfo { Id = "FBE3EE50747CB892", FullName = "Lunakitty" },
                new PlayerInfo { Id = "339E0D392565DC39", FullName = "kishark" },
                new PlayerInfo { Id = "660814E013F31EFA", FullName = "HOLLOWZZGT" },
                new PlayerInfo { Id = "2E408ED946D55D51", FullName = "Haunted" },
                new PlayerInfo { Id = "D345FE394607F946", FullName = "Bzzz the 18th" },
                new PlayerInfo { Id = "41988726285E534E", FullName = "Colussus" },
                new PlayerInfo { Id = "BC99FA914F506AB8", FullName = "Lemming 1" },
                new PlayerInfo { Id = "3A16560CA65A51DE", FullName = "Lemming 2" },
                new PlayerInfo { Id = "59F3FE769DE93AB9", FullName = "Lemming 3" },
                new PlayerInfo { Id = "608E4B07DBEFC690", FullName = "BLU" },
                new PlayerInfo { Id = "FB5FCEBC4A0E0387", FullName = "PepsiDee" }
            };

            foreach (var player in players)
            {
                string[] nameParts = player.FullName.Split(new[] { ' ' }, 2);
                player.Name = nameParts[0];

                if (!groupedPlayers.ContainsKey(player.Name))
                {
                    groupedPlayers[player.Name] = new List<PlayerInfo>();
                    expandedGroups[player.Name] = false;
                }
                groupedPlayers[player.Name].Add(player);
            }
        }

        void OnGUI()
        {
            if (showGUI)
            {
                GUI.Box(new Rect(30, 30, 480, 600), "Ghosts GUI Temp [CTRL + P = Close]");
                GUI.color = Color.grey;
                GUI.contentColor = Color.white;

                if (!showWellKnownPage)
                {
                    DrawMainPage();
                }
                else
                {
                    DrawWellKnownPage();
                }

                // closing gui
                Event e = Event.current;
                if (e != null && e.isKey && e.keyCode == KeyCode.P && e.control)
                {
                    showGUI = false;
                }
            }
            else
            {
                GUI.Label(new Rect(10, 10, 200, 20), "CTRL + C to open GUI");
                Event e = Event.current;
                if (e != null && e.isKey && e.keyCode == KeyCode.C && e.control)
                {
                    showGUI = true;
                }
            }
        }

        void DrawMainPage()
        {
            GUI.Label(new Rect(50, 80, 100, 20), "PlayFab ID:");
            playfabId = GUI.TextField(new Rect(160, 80, 300, 20), playfabId);

            GUI.Label(new Rect(50, 110, 100, 20), "Steam Ticket:");
            steamTicket = GUI.TextField(new Rect(160, 110, 300, 20), steamTicket);

            if (GUI.Button(new Rect(160, 140, 200, 30), "Authenticate"))
            {
                StartCoroutine(AuthenticateWithPlayFab());
            }

            if (GUI.Button(new Rect(160, 180, 200, 30), "Well Known"))
            {
                showWellKnownPage = true;
                selectedPlayerId = "";
                accountInfoText = "";
            }

            GUI.Label(new Rect(50, 220, 100, 20), "Account Info:");
            accountInfoScrollPosition = GUI.BeginScrollView(new Rect(50, 240, 440, 360), accountInfoScrollPosition, new Rect(0, 0, 420, Mathf.Max(360, GUI.skin.label.CalcHeight(new GUIContent(accountInfoText), 420))));
            GUI.TextArea(new Rect(0, 0, 420, Mathf.Max(360, GUI.skin.label.CalcHeight(new GUIContent(accountInfoText), 420))), accountInfoText);
            GUI.EndScrollView();
        }

        void DrawWellKnownPage()
        {
            if (GUI.Button(new Rect(50, 60, 100, 30), "Back"))
            {
                showWellKnownPage = false;
            }

            GUI.Label(new Rect(160, 60, 100, 20), "Steam Ticket:");
            steamTicket = GUI.TextField(new Rect(260, 60, 200, 20), steamTicket);

            if (GUI.Button(new Rect(160, 90, 200, 30), "Authenticate"))
            {
                if (selectedPlayerId != "")
                {
                    StartCoroutine(AuthenticateWithPlayFab());
                }
            }

            wellKnownScrollPosition = GUI.BeginScrollView(new Rect(50, 130, 200, 450), wellKnownScrollPosition, new Rect(0, 0, 180, CalculateTotalHeight()));
            float yOffset = 0;
            foreach (var group in groupedPlayers)
            {
                if (GUI.Button(new Rect(0, yOffset, 180, 25), group.Key))
                {
                    if (group.Value.Count > 1)
                    {
                        expandedGroups[group.Key] = !expandedGroups[group.Key];
                    }
                    else
                    {
                        selectedPlayerId = group.Value[0].Id;
                        accountInfoText = "";
                    }
                }
                yOffset += 30;

                if (expandedGroups[group.Key])
                {
                    foreach (var player in group.Value)
                    {
                        if (GUI.Button(new Rect(20, yOffset, 160, 25), player.FullName))
                        {
                            selectedPlayerId = player.Id;
                            accountInfoText = "";
                        }
                        yOffset += 30;
                    }
                }
            }
            GUI.EndScrollView();

            GUI.Label(new Rect(260, 130, 100, 20), "Account Info:");
            accountInfoScrollPosition = GUI.BeginScrollView(new Rect(260, 150, 200, 430), accountInfoScrollPosition, new Rect(0, 0, 180, Mathf.Max(430, GUI.skin.label.CalcHeight(new GUIContent(accountInfoText), 180))));
            GUI.TextArea(new Rect(0, 0, 180, Mathf.Max(430, GUI.skin.label.CalcHeight(new GUIContent(accountInfoText), 180))), accountInfoText);
            GUI.EndScrollView();
        }

        float CalculateTotalHeight()
        {
            float totalHeight = 0;
            foreach (var group in groupedPlayers)
            {
                totalHeight += 30; // Height for group name
                if (expandedGroups[group.Key])
                {
                    totalHeight += group.Value.Count * 30; // Height for each player in expanded group
                }
            }
            return totalHeight;
        }

        IEnumerator AuthenticateWithPlayFab()
        {
            if (string.IsNullOrEmpty(playfabId) && string.IsNullOrEmpty(selectedPlayerId))
            {
                accountInfoText = "Please enter a PlayFab ID or select a player.";
                yield break;
            }

            if (string.IsNullOrEmpty(steamTicket))
            {
                accountInfoText = "Please enter a Steam Ticket.";
                yield break;
            }

            string loginUrl = $"https://{PlayFabTitleId}.playfabapi.com/Client/LoginWithSteam";
            string loginPayload = $"{{\"SteamTicket\":\"{steamTicket}\",\"TitleId\":\"{PlayFabTitleId}\",\"CreateAccount\":true}}";

            using (UnityWebRequest loginRequest = new UnityWebRequest(loginUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(loginPayload);
                loginRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                loginRequest.downloadHandler = new DownloadHandlerBuffer();
                loginRequest.SetRequestHeader("Content-Type", "application/json");

                yield return loginRequest.SendWebRequest();

                if (loginRequest.result != UnityWebRequest.Result.Success)
                {
                    accountInfoText = $"Login Error: {loginRequest.error}";
                    yield break;
                }

                string loginResponse = loginRequest.downloadHandler.text;
                JObject loginJson = JObject.Parse(loginResponse);
                sessionTicket = loginJson["data"]?["SessionTicket"]?.ToString();

                if (string.IsNullOrEmpty(sessionTicket))
                {
                    accountInfoText = "Failed to extract session ticket from login response";
                    yield break;
                }

                string targetPlayFabId = !string.IsNullOrEmpty(selectedPlayerId) ? selectedPlayerId : playfabId;
                yield return StartCoroutine(GetAccountInfo(targetPlayFabId, sessionTicket));
            }
        }

        IEnumerator GetAccountInfo(string playFabId, string sessionTicket)
        {
            string accountInfoUrl = $"https://{PlayFabTitleId}.playfabapi.com/Client/GetAccountInfo";
            string accountInfoPayload = $"{{\"PlayFabId\":\"{playFabId}\"}}";

            using (UnityWebRequest accountInfoRequest = new UnityWebRequest(accountInfoUrl, "POST"))
            {
                byte[] accountInfoBodyRaw = Encoding.UTF8.GetBytes(accountInfoPayload);
                accountInfoRequest.uploadHandler = new UploadHandlerRaw(accountInfoBodyRaw);
                accountInfoRequest.downloadHandler = new DownloadHandlerBuffer();
                accountInfoRequest.SetRequestHeader("Content-Type", "application/json");
                accountInfoRequest.SetRequestHeader("X-Authorization", sessionTicket);

                yield return accountInfoRequest.SendWebRequest();

                if (accountInfoRequest.result != UnityWebRequest.Result.Success)
                {
                    accountInfoText = $"Account Info Error: {accountInfoRequest.error}";
                    yield break;
                }

                string accountInfoResponse = accountInfoRequest.downloadHandler.text;

                try
                {
                    JObject accountInfo = JObject.Parse(accountInfoResponse);
                    JToken accountInfoData = accountInfo["data"]?["AccountInfo"];

                    if (accountInfoData != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"PlayFab ID: {accountInfoData["PlayFabId"]}");
                        sb.AppendLine($"Created: {accountInfoData["Created"]}");

                        JToken titleInfo = accountInfoData["TitleInfo"];
                        if (titleInfo != null)
                        {
                            sb.AppendLine("Title Info:");
                            sb.AppendLine($"  Display Name: {titleInfo["DisplayName"]}");
                            sb.AppendLine($"  Origination: {titleInfo["Origination"]}");
                            sb.AppendLine($"  Created: {titleInfo["Created"]}");
                            sb.AppendLine($"  Last Login: {titleInfo["LastLogin"]}");
                            sb.AppendLine($"  First Login: {titleInfo["FirstLogin"]}");
                            sb.AppendLine($"  Is Banned: {titleInfo["isBanned"]}");

                            JToken titlePlayerAccount = titleInfo["TitlePlayerAccount"];
                            if (titlePlayerAccount != null)
                            {
                                sb.AppendLine("  Title Player Account:");
                                sb.AppendLine($"    Id: {titlePlayerAccount["Id"]}");
                                sb.AppendLine($"    Type: {titlePlayerAccount["Type"]}");
                            }
                        }

                        JToken steamInfo = accountInfoData["SteamInfo"];
                        if (steamInfo != null)
                        {
                            sb.AppendLine("Steam Info:");
                            sb.AppendLine($"  Steam ID: {steamInfo["SteamId"]}");
                            sb.AppendLine($"  Steam Name: {steamInfo["SteamName"]}");
                            sb.AppendLine($"  Steam Country: {steamInfo["SteamCountry"]}");
                            sb.AppendLine($"  Steam Currency: {steamInfo["SteamCurrency"]}");
                        }

                        accountInfoText = sb.ToString();
                    }
                    else
                    {
                        accountInfoText = "Failed to extract account info from response.";
                    }
                }
                catch (JsonException ex)
                {
                    accountInfoText = $"Error parsing account info: {ex.Message}";
                }
            }
        }
    }
}