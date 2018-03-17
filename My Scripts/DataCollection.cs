using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReportFields
{
    public const string playerName = "playerName";
    public const string gameTime = "gameTime";
    public const string startTime = "startTime";
    public const string endTime = "endTime";
    public const string armUseCount = "armUseCount";
    public const string draggingRequest = "draggingRequest";
    public const string dragStartTime = "dragStartTime";
    public const string draggedPlayerName = "draggedPlayerName";
    public const string gasHit = "gasHit";
    public const string hitTime = "hitTime";
    public const string hittingPlayerName = "hittingPlayerName";
    public const string airRequest = "airRequest";
    public const string requestTime = "requestTime";
    public const string averageAir = "averageAir";
    public const string stagesCompletedCount = "stagesCompletedCount";
}
public class DataCollection : MonoBehaviour {

    private string filePath;
    private XmlDocument gameReport;

	// Use this for initialization
	 void Start () {

        filePath = ""+Directory.GetCurrentDirectory() + "\\GameReport.xml";
        gameReport = new XmlDocument();
        gameReport.Load(filePath);
       
	}




    /******************************************/
    // initializing game report xml with players reports
    /******************************************/
    private void CheckCanUpdateFile()
    {
               
            if (Time.unscaledTime > 10)
            {
                CreatePlayerReportXml();
            }
        
    }

    private void CreatePlayerReportXml()
    {
        AddReportsForPlayers();
        SaveDocument();
    }

    private XmlNode AddReportsForPlayers()
    {
        XmlNode playerReport = gameReport.SelectSingleNode("/gameReport/playerReport");
        return gameReport.DocumentElement.AppendChild(playerReport.Clone());

    }


    /***********************************************/

       
    public void AddPlayerName(string name)
    {
        XmlNode newReport = AddReportsForPlayers();
        newReport.FirstChild.InnerText = name;

        SaveDocument();
    }
    public void AddGameTime(string playerName, string endTime)
    {
        XmlNodeList playerGameTime = gameReport.SelectNodes("/gameReport/playerReport/gameTime");

        foreach (XmlNode node in playerGameTime)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                node.FirstChild.InnerText = endTime;
                SaveDocument();
                break;
            }
        }

    }
    public void AddArmUseCount(string playerName)
    {
        XmlNodeList playerArmUseCount = gameReport.SelectNodes("/gameReport/playerReport/armUseCount");

        foreach (XmlNode node in playerArmUseCount)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                node.InnerText = string.Format("{0}",int.Parse(node.InnerText)+1);
                SaveDocument();
                break;
            }
        }


    }
    public void AddDraggingRequest(string playerName, string requestTime)
    {
        XmlNodeList playerDraggingRequest = gameReport.SelectNodes("/gameReport/playerReport/draggingRequest");

        foreach (XmlNode node in playerDraggingRequest)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                if (node.FirstChild.InnerText == "")
                {
                    node.FirstChild.InnerText = requestTime;

                }
                else
                {
                   XmlNode newRequest = node.AppendChild(node.FirstChild.Clone());
                   newRequest.InnerText = requestTime;
                }
                SaveDocument();
                break;
            }
        }
    }
    public void AddDraggingOther(string playerName, string time, string draggedPlayerName)
    {
        XmlNodeList playerDraggingOther = gameReport.SelectNodes("/gameReport/playerReport/draggingOther");
        foreach (XmlNode node in playerDraggingOther)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                if (node.FirstChild.InnerText == "")
                {
                    node.FirstChild.InnerText = time;
                    node.FirstChild.NextSibling.InnerText = draggedPlayerName; 

                }
                else
                {
                    XmlNode newDragTime = node.AppendChild(node.FirstChild.Clone());                  
                    XmlNode newDraggedPlayerName = node.AppendChild(node.FirstChild.NextSibling.Clone());
                    newDragTime.InnerText = time;
                    newDraggedPlayerName.InnerText = draggedPlayerName;
                }
                SaveDocument();
                break;
            }
        }
    }
    public void AddGasHit(string playerName, string hitTime, string hittingPlayerName)
    {
        XmlNodeList playerGasHit = gameReport.SelectNodes("/gameReport/playerReport/gasHit");
        foreach (XmlNode node in playerGasHit)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                if (node.FirstChild.InnerText == "")
                {
                    node.FirstChild.InnerText = hitTime;
                    node.FirstChild.NextSibling.InnerText = hittingPlayerName;

                }
                else
                {
                    XmlNode newDragTime = node.AppendChild(node.FirstChild.Clone());
                    XmlNode newDraggedPlayerName = node.AppendChild(node.FirstChild.NextSibling.Clone());
                    newDragTime.InnerText = hitTime;
                    newDraggedPlayerName.InnerText = hittingPlayerName;
                }
                SaveDocument();
                break;
            }
        }
    }
    public void AddAirRequest(string playerName, string requestTime)
    {
        XmlNodeList playerAirRequest = gameReport.SelectNodes("/gameReport/playerReport/airRequest");

        foreach (XmlNode node in playerAirRequest)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                if (node.FirstChild.InnerText == "")
                {
                    node.FirstChild.InnerText = requestTime;

                }
                else
                {
                    XmlNode newRequest = node.AppendChild(node.FirstChild.Clone());
                    newRequest.InnerText = requestTime;
                }
                SaveDocument();
                break;
            }
        }
    }
    public void AddEmojiUse(string playerName, string time, string type)
    {
        XmlNodeList playerEmojiUse = gameReport.SelectNodes("/gameReport/playerReport/emojiUse");
        foreach (XmlNode node in playerEmojiUse)
        {
            if (node.ParentNode.FirstChild.InnerText == playerName)
            {
                if (node.FirstChild.InnerText == "")
                {
                    node.FirstChild.InnerText = time;
                    node.FirstChild.NextSibling.InnerText = type;

                }
                else
                {
                    XmlNode newDragTime = node.AppendChild(node.FirstChild.Clone());
                    XmlNode newDraggedPlayerName = node.AppendChild(node.FirstChild.NextSibling.Clone());
                    newDragTime.InnerText = time;
                    newDraggedPlayerName.InnerText = type;
                }
                SaveDocument();
                break;
            }
        }
    }
    public static void AddAverageAir(string playerName, string air)
    { }
    public static void AddStagesCompletedCount()
    { }
    private static void OnPlayerConnected(NetworkPlayer player)
    {       

    }
    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        
    }
    private XmlNode FindPlayerNode(string playerName)
    {
        XmlNode root = gameReport.DocumentElement;
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.ChildNodes[0].Name == "playerName")
            {
                XmlNode temp = node.ChildNodes[0];
                if (temp.InnerText == playerName)
                    return temp;
            }
        }

        return null;
    }
    public void AddValueToPlayerReport(string playerName,string field,string value)
    {
         XmlNode playerNode = gameReport.SelectSingleNode("/gameReport/playerReport/" + playerName + "/" +field);
        playerNode.InnerText = value;
    }


    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (GetComponent<NetworkIdentity>().isServer)
        SaveDocument();
    }
    private void SaveDocument()
    {
        gameReport.Save(filePath);
    }

}
