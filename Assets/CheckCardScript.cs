using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CheckCardScipt : NetworkBehaviour{

    public bool IsPair(int a , int b)
    {
        if(a/4 == b/4)
        {
            return true;
        }

        return false;
        
    }

    public bool IsTriple(int a, int b , int c)
    {
        if (a / 4 == b / 4 && b / 4 == c / 4)
        {
            return true;
        }

        return false;


    }

    public bool IsQuad(int a, int b, int c , int d, int e)
    {
        if (a / 4 == b / 4 && b / 4 == c / 4 && c / 4 == d / 4)
        {
            return true;
        }else if (b / 4 == c / 4 && c / 4 == d / 4 && d / 4 == e / 4)
        {
            return true;
        }

        return false;
    }

    public bool IsFullhouse(int a, int b , int c , int d , int e)
    {


        if (IsPair(a, b))   // Verify that the first two are twoPair
        {
            if (IsTriple(c, d, e)) // Verify whether the three are Triple
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (IsTriple(a, b, c)) // Verify that the first three are Triple
        {
            if (IsPair(d, e)) // Verify that the two are twoPair
            {
                return true;
            }
            else
            {
                return false;
            }
        }

         return false;
       
    }

    public bool IsStraight(int a, int b, int c, int d, int e)
    {
        if (e/4 - d/4 == 1 && d/4 - c/4 == 1 && c/4 - b/4 == 1 && b/4 - a/4 == 1) //Verify adjacent straights
        {
            return true;
        }
        else if (e / 4 - d / 4 == 1 && d / 4 - c / 4 == 1 && c / 4 - b / 4 == 1 && a / 4 == 0) //Verify across a card such as A 10 J Q K
        {
            if(e/4 - a/4 == 12) //Verification crossing
            {
                return true; 
            }
            return false;
        }
        else if (e / 4 - d / 4 == 1 && d / 4 - c / 4 == 1 && a / 4 == 0 && b/4 ==1) //Verify that spanning two cards, such as A 2 J Q K
        {
            if(e/4 - a/4 == 12) //Verification crossing
            {
                return true;
            }
            return false;
        }
        else if (e / 4 - d / 4 == 1 && a / 4 == 0 && b / 4 == 1 && c / 4 == 2) //Verify spanning three cards such as A 2 3 Q K
        {
            if (e / 4 - a / 4 == 12) //Verification crossing
            {
                return true;
            }
            return false;
        }else if (e / 4 == 12 && a / 4 == 0 && b / 4 == 1 && c / 4 == 2 && d / 4 == 3) //Verify across four cards such as A 2 3 4 K
        {
            if (e / 4 - a / 4 == 12) //Verification crossing
            {
                return true;
            }
            return false;
        }

            return false;
    }
    public bool IsStraightFlush(int a, int b, int c, int d, int e)
    {
        if(a%4 == b%4 && b%4 == c%4 && c%4 == d%4 && d%4 == e % 4)
        {
            if (IsStraight(a,b,c,d,e))
            {
                return true;
            }

            return false;
        }

        return false;
        
    }

    public String Check5(int a , int b , int c , int d , int e) 
    {
        if (IsStraightFlush(a, b, c, d, e)) return "Straight flush";
        if (IsQuad(a, b, c, d, e)) return "Iron branch";
        if (IsStraight(a, b, c, d, e)) return "Straight";
        if (IsFullhouse(a, b, c, d, e)) return "gourd";





        return null;
    }






















}
