using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;

public class Manager
{
    private static Manager instance = null;

    public static Manager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new Manager();
            }
            return instance;
        }
    }

    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;


    public string UserId => user.UserId;

    public Action<bool> LoginState;

    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        auth.StateChanged += Onchanged;
    }

    public void Onchanged(object sender, EventArgs e)
    {
        if(auth.CurrentUser != user)
        {
            bool signed = (auth.CurrentUser != user && auth.CurrentUser != null);
            if(!signed && user != null)
            {
                Debug.LogError("�α׾ƿ�");
                LoginState.Invoke(false);
            }
            user = auth.CurrentUser;
            if (signed)
            {
                Debug.LogError("�α���");
                LoginState.Invoke(true);
            }
        }
    }

    public void Create(string email, string password, string name)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("ȸ������ ���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("ȸ������ ����");
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;

            //Data(user. UserId);
            Data(UserId, name);
            Debug.LogError("ȸ������ �Ϸ�");

        });
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("�α��� ���");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("�α��� ����");
                return;
            }
        });
    }
    public void Logout()
    {
        auth.SignOut();
        Debug.LogError("�α׾ƿ�");
    }

    public void Data(string userId, string name)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "name", name },
            { "Stage", 0 },
        };

        db.Collection("users").Document(userId).SetAsync(data).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("�������");
                return;
            }

            Debug.Log("���� ����");
        });
    }

    public void Score(string userId)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("������ �ҷ����� ����.");
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                int currentScore = snapshot.GetValue<int>("Stage");
                int newScore = currentScore + 1;

                Dictionary<string, object> newData = new Dictionary<string, object>
                {
                    { "Stage", newScore }
                };

                userRef.UpdateAsync(newData).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted || updateTask.IsCanceled)
                    {
                        Debug.LogError("���������� ������Ʈ ����");
                        return;
                    }

                    Debug.Log("���� ��������: " + newScore);
                });
            }
        });
    }
}
