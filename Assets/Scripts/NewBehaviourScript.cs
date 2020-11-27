using UnityEngine;
using System.Text;
using StarX.DotNetClient;
using System.IO;
using Google.Protobuf;
using Testdata;

public class NewBehaviourScript : MonoBehaviour
{
    private static StarXClient client;
    // Use this for initialization
    void Start () {
        StarXClient client = new StarXClient();
        client.Init("127.0.0.1", 3250, () =>
        {
            Debug.Log("init client callback");
            client.Connect((data) =>
            {
                Debug.Log("connect client callback");
                
                // 服务器主动推送消息
                client.On("onNewUser", (m) =>
                {
                    NewUser nu = NewUser.Parser.ParseFrom(m);
                    Debug.Log("onNewUser: " + nu.Content);
                });

                //服务器主动推送消息
                client.On("onMembers", (m) =>
                {
                    AllMembers am = AllMembers.Parser.ParseFrom(m);
                    Debug.Log("onMembers: " + am.Members);
                });
                
                client.On("onMessage", (m) =>
                {
                    Testdata.UserMessage um = UserMessage.Parser.ParseFrom(m);
                    Debug.Log("onMessage: " + um.Name +" : " + um.Content);
                });
                
                //客户端请求，服务器回答
                Testdata.Ping first = new Testdata.Ping{Content = "hello"};
                client.Request("room.join", first.ToByteArray(), (resp) =>
                {
                    JoinResponse jp = JoinResponse.Parser.ParseFrom(resp);
                    Debug.Log("room.join response: " + jp.Result);
                });
                
                // 客户端推送，没有回消息
                UserMessage msg = new UserMessage{Name = "小明",Content = "我来了"};
                client.Notify("room.message",msg.ToByteArray());
                
            });
        });
    }
  
    
}
