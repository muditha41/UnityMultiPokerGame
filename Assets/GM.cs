using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class GM : NetworkBehaviour
{


    const int NUMBER_OF_CARDS = 32;
    int[] Cards = new int[NUMBER_OF_CARDS];
    int[] playerlead;
    public List<Player> allPlayer = new List<Player>();

    public Round round = Round.non;
    public Process process = Process.start;
    public bool firstlead = true;
    public bool roundfirstlead = true;
    GameObject CardLeadShow;
    SyncListCardItem CardList = new SyncListCardItem();



    public enum Round
    {
        non,
        player1,
        player2,
        player3,
        player4
    }

    public enum Process
    {
        start,
        waitLogin,
        decidePlayer,
        shuffle,
        choicefirstplayer,
        p1Action,
        p2Action,
        p3Action,
        p4Action,
        checkWin,
        end,
    }

    //Player login

    public void Login(Player player)
    {
        allPlayer.Add(player);


        if (allPlayer.Count == 4)//
            process = Process.shuffle;//
    }


    //Verify the rules of the card

    public void CheckCard(ArrayList playerCards,int count)
    {

        if (count < 1 || count > 5 || count == 4) LeadError(); //The number of cards is wrong.

        playerlead = new int[playerCards.Count];

        //First card
        if (roundfirstlead)
        {
            //Verify that the card contains plum 3
            if (!CheckPlum3(playerCards))
            {
                foreach (Player pl in allPlayer)
                {
                    if (pl.process == Player.Process.action) pl.TipMsg = "this error Please come out first plum 3";
                }
            }


            //Verification deck
            if (GmCheck(playerCards, count))
            {


                CardLeadShow.SetActive(true);

                for (int i = 0; i > 5; i++)
                {
                    if (i >= playerCards.Count) GameObject.Find("x" + (i + 1)).GetComponent<SpriteRenderer>().sprite = null;
                    GameObject.Find("x"+(i+1)).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Convert.ToString(playerCards[i]));
                    
                }
                


                if (allPlayer[0].process == Player.Process.action)
                {
                    process = Process.p2Action;
                }
                else if (allPlayer[1].process == Player.Process.action)
                {
                    process = Process.p3Action; 
                }
                else if (allPlayer[2].process == Player.Process.action)
                {
                    process = Process.p4Action;
                }
                else if (allPlayer[3].process == Player.Process.action)
                {
                    process = Process.p1Action;
                }
            }
            else
            {
                LeadError();
            }
            
        }


    }


    void Start()
    {
        process = Process.waitLogin;
        Instantiate(CardLeadShow, transform.position, transform.rotation);
        CardLeadShow.SetActive(false);


    }

    void Update()
    {

        switch (process)
        {
            case Process.waitLogin:
                foreach (Player pl in allPlayer)
                    pl.SysMsg = "Waiting for players to connect... current number:" + allPlayer.Count;

                break;

            case Process.shuffle:



                foreach (Player pl in allPlayer)
                    pl.SysMsg = "The game begins to start licensing";



                // Rework card
                for (int i = 0; i < NUMBER_OF_CARDS; i++)
                {
                    Cards[i] = i;
                }

                // Shuffle
                for (int i = 0; i < NUMBER_OF_CARDS; i++)
                {
                    int swap_index = UnityEngine.Random.Range(0, 31);
                    int t = Cards[i];
                    Cards[i] = Cards[swap_index];
                    Cards[swap_index] = t;
                }

                // Building a library

                for (int i = 0; i < NUMBER_OF_CARDS; i++)
                {
                    CardStruct card = new CardStruct();
                    card.UID = Cards[i];
                    CardList.Add(card);

                    
                }

                //Create a library and issue a card
                for (int i = 0; i < 8; i++)
                {
                    if (i == 3)
                    {

                    }
                    

                    allPlayer[0].Server_AddCard(CardList.GetItem(i));
                    allPlayer[1].Server_AddCard(CardList.GetItem(8 + i));
                    allPlayer[2].Server_AddCard(CardList.GetItem(16 + i));
                    allPlayer[3].Server_AddCard(CardList.GetItem(24 + i));

                }


                allPlayer[0].IsGameStart = true;
                allPlayer[1].IsGameStart = true;
               allPlayer[2].IsGameStart = true;
                allPlayer[3].IsGameStart = true;

                process = Process.decidePlayer;

                break;

            case Process.decidePlayer:

                foreach (Player pl in allPlayer)
                {
                    pl.SysMsg = "Waiting time Ready to start Looking for Plum 3 players";
                    
                }

                Load();
                
                for (int i = 0; i < NUMBER_OF_CARDS / 4; i++)
                {

                    if (allPlayer[0].HaveCards.GetItem(i).UID == 8) // me change 8 to 4 all of this
                    {
                        process = Process.p1Action;
                    }
                    else if (allPlayer[1].HaveCards.GetItem(i).UID == 8)
                    {
                        process = Process.p2Action;
                    }
                    else if (allPlayer[2].HaveCards.GetItem(i).UID == 8)
                    {
                        process = Process.p3Action;
                    }
                    else if (allPlayer[3].HaveCards.GetItem(i).UID == 8)
                    {
                        process = Process.p4Action;
                    }

                }
                 break;

            case Process.p1Action:
                allPlayer[0].SysMsg = "It's your turn!";
                allPlayer[0].SetProcess(Player.Process.action);
                allPlayer[1].SysMsg = "Waiting each other...";
                allPlayer[1].SetProcess(Player.Process.wait);
                allPlayer[2].SysMsg = "Waiting each other...";
                allPlayer[2].SetProcess(Player.Process.wait);
                allPlayer[3].SysMsg = "Waiting each other...";
                allPlayer[3].SetProcess(Player.Process.wait);
                break;

            case Process.p2Action:
                allPlayer[1].SysMsg = "It's your turn!";
                allPlayer[1].SetProcess(Player.Process.action);
                allPlayer[0].SysMsg = "Waiting each other...";
                allPlayer[0].SetProcess(Player.Process.wait);
                allPlayer[2].SysMsg = "Waiting each other...";
                allPlayer[2].SetProcess(Player.Process.wait);
                allPlayer[3].SysMsg = "Waiting each other...";
                allPlayer[3].SetProcess(Player.Process.wait);
                break;

            case Process.p3Action:
                allPlayer[2].SysMsg = "It's your turn!";
                allPlayer[2].SetProcess(Player.Process.action);
                allPlayer[0].SysMsg = "Waiting each other...";
                allPlayer[0].SetProcess(Player.Process.wait);
                allPlayer[1].SysMsg = "Waiting each other...";
                allPlayer[1].SetProcess(Player.Process.wait);
                allPlayer[2].SysMsg = "Waiting each other...";
                allPlayer[2].SetProcess(Player.Process.wait);
                break;

            case Process.p4Action:
                allPlayer[3].SysMsg = "It's your turn!";
                allPlayer[3].SetProcess(Player.Process.action);
                allPlayer[1].SysMsg = "Waiting each other...";
                allPlayer[1].SetProcess(Player.Process.wait);
                allPlayer[2].SysMsg = "Waiting each other...";
                allPlayer[2].SetProcess(Player.Process.wait);
                allPlayer[0].SysMsg = "Waiting each other...";
                allPlayer[0].SetProcess(Player.Process.wait);
                break; 

            case Process.checkWin:
                switch (round)
                {
                    case Round.player1:
                        allPlayer[0].SysMsg = "Winner";
                        allPlayer[1].SysMsg = "Loser";
                        break;
                    case Round.player2:
                        allPlayer[1].SysMsg = "Winner";
                        allPlayer[0].SysMsg = "Loser";
                        break;
                }
                allPlayer[0].SetProcess(Player.Process.end);
                allPlayer[1].SetProcess(Player.Process.end);
                process = Process.end;
                break;
        }
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(3);    //Pay attention to the waiting time
    }

    //Prompt players not to play cards
    public void LeadError()
    {
        foreach (Player pl in allPlayer)
        {
            if (pl.process == Player.Process.action)
            {
                pl.TipMsg = "Does not match the card";
            }
        }
    }


    //Verify plum 3

    public bool CheckPlum3(ArrayList playerCards)
    {
        for(int i = 0; i < playerCards.Count; i++)
        {
            if ((Int32)playerCards[i] == 8) // me change to 8 to 4
            {
                return true;
            }
            
        }
        return false;
    }

    //Verification card combination

    public bool GmCheck(ArrayList playerCards, int count)
    {

        CheckCardScipt checkCardScipt = new CheckCardScipt();

        for (int i = 0; i < playerlead.Length; i++)
        {
            playerlead[i] = (int)playerCards[i];
        }


        if (count == 5)
        {
            if (checkCardScipt.Check5(playerlead[0], playerlead[1], playerlead[2], playerlead[3], playerlead[4]).Equals("Straight flush"))
            {
                return true;
            }
            if (checkCardScipt.Check5(playerlead[0], playerlead[1], playerlead[2], playerlead[3], playerlead[4]).Equals("Iron branch"))
            {
                return true;
            }
            if (checkCardScipt.Check5(playerlead[0], playerlead[1], playerlead[2], playerlead[3], playerlead[4]).Equals("Hulu"))
            {
                return true;
            }
            if (checkCardScipt.Check5(playerlead[0], playerlead[1], playerlead[2], playerlead[3], playerlead[4]).Equals("Straight"))
            {
                return true;
            }

            return false;
        }else if(count == 2)
        {
            if (checkCardScipt.IsPair(playerlead[0], playerlead[1])) return true;
            return false;
        }else if(count == 1)
        {
            return true;
        }
        return false;
    }



}