using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
  [SerializeField]
  Transform character;

  AudioSource audioSource;

//   [SerializeField]
//   AudioClip audio;

  float tgt = 5f;
  float speed;

  void Start() {
    Application.runInBackground = true;

    StartCoroutine(StreamPlayAudioFile());

  }

//   void Start() {
//     audo.setau
//   }

  void Update() {
    speed = Mathf.Lerp (tgt, speed, Time.deltaTime);

    transform.Rotate (Vector3.up, speed * Time.deltaTime);

    var animator = character.GetComponent<Animator> ();
    animator.SetFloat ("Speed", speed);
  }

  //ソース指定し音楽流す
  //音楽ファイルロード
  IEnumerator StreamPlayAudioFile()
  {
    WWW www = new WWW("file:///Users/tomiyamaryouhei/Documents/www/bonos/3d-avatar/Assets/UnityChan/Voice/univ0024.wav");
    //読み込み完了まで待機
    yield return www;

    audioSource=GetComponent<AudioSource>();
    audioSource.clip = www.GetAudioClip(true, true);
    audioSource.Play();
  }

}