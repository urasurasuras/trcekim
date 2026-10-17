// <copyright file="EncryptionHelper.cs" company="GinkoSolutions.com">
// Copyright (c) 2011 All Right Reserved
// </copyright>
// <author>Sean Greasley</author>
// <email>sean@ginkosolutions.com</email>
// <summary>Enables simple TripleDES encryption with or without hashing</summary>
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;

/// <summary>
/// Enables simple TripleDES encryption with or without hashing
/// </summary>
public class EncryptionHelper
{
    /// <summary>
    /// Encryption key. Used to encrypt and decrypt.
    /// </summary>
    private static readonly string encryptionKey = "YOURSECRETKEY";

    public EncryptionHelper() { }

    /// <summary>
    /// Encrypt text string
    /// </summary>
    /// <param name="toEncrypt">The string of data to encrypt</param>
    /// <param name="useHashing">Weather hashing is used or not</param>
    /// <returns>An encrypted string</returns>
    public static string Encrypt(string toEncrypt, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

        // If hashing use get hashcode regards to your key
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptionKey));
            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(encryptionKey);

        // Set the secret key for the tripleDES algorithm
        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        // Transform the specified region of bytes array to resultArray
        ICryptoTransform cTransform = tdes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();

        // Return the encrypted data into unreadable string format
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    /// Decrypts a text string
    /// </summary>
    /// <param name="cipherString">The encrypted string</param>
    /// <param name="useHashing">Weather hashing is used or not</param>
    /// <returns>Decrypted text string</returns>
    public static string Decrypt(string cipherString, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace(' ', '+'));

        if (useHashing)
        {
            // If hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(encryptionKey));
            hashmd5.Clear();
        }
        else
        {
            // If hashing was not implemented get the byte code of the key
            keyArray = UTF8Encoding.UTF8.GetBytes(encryptionKey);
        }

        // Set the secret key for the tripleDES algorithm
        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = keyArray;
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tdes.Clear();

        // Return the Clear decrypted TEXT
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}