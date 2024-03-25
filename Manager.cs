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
                Debug.LogError("로그아웃");
                LoginState.Invoke(false);
            }
            user = auth.CurrentUser;
            if (signed)
            {
                Debug.LogError("로그인");
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
                Debug.LogError("회원가입 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("회원가입 실패");
                return;
            }

            AuthResult authResult = task.Result;
            FirebaseUser newUser = authResult.User;

            //Data(user. UserId);
            Data(UserId, name);
            Debug.LogError("회원가입 완료");

        });
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패");
                return;
            }
        });
    }
    public void Logout()
    {
        auth.SignOut();
        Debug.LogError("로그아웃");
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
                Debug.LogError("저장실패");
                return;
            }

            Debug.Log("저장 성공");
        });
    }

    public void Score(string userId)
    {
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("데이터 불러오기 실패.");
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
                        Debug.LogError("스테이지를 업데이트 실패");
                        return;
                    }

                    Debug.Log("현재 스테이지: " + newScore);
                });
            }
        });
    }
}
