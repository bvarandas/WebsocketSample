<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Gradual.Spider.PositionClient.Monitor.Test.Www._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Modify this template to jump-start your ASP.NET application.</h2>
            </hgroup>
            <p>
                To learn more about ASP.NET, visit <a href="http://asp.net" title="ASP.NET Website">http://asp.net</a>.
                The page features <mark>videos, tutorials, and samples</mark> to help you get the most from ASP.NET.
                If you have any questions about ASP.NET visit
                <a href="http://forums.asp.net/18.aspx" title="ASP.NET Forum">our forums</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>We suggest the following:</h3>
    <ol class="round">
        <li class="one">
            <h5>Getting Started</h5>
            ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245146">Learn more…</a>
        </li>
        <li class="two">
            <h5>Add NuGet packages and jump-start your coding</h5>
            NuGet makes it easy to install and update free libraries and tools.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245147">Learn more…</a>
        </li>
        <li class="three">
            <h5>Find Web Hosting</h5>
            You can easily find a web hosting company that offers the right mix of features and price for your applications.
            <a href="http://go.microsoft.com/fwlink/?LinkId=245143">Learn more…</a>
        </li>
    </ol>
    <script type="text/javascript" src="jquery.js"></script>
<script type="text/javascript">
    var _NoSupportMessage = "Your browser cannot support WebSocket!";
    var _Ws;

    function AppendMessage(pMessage)
    {
        $('#htmlBody').append(message);
    }

    function ConnectSocketServer()
    {
        var lSupport = "MozWebSocket" in window ? 'MozWebSocket' : ("WebSocket" in window ? 'WebSocket' : null);

        if (lSupport == null) {
            AppendMessage("* " + _NoSupportMessage + "<br />");
            return;
        }

        AppendMessage("* Connnectiong to server....<br />");

        _Ws = new window[lSupport]("ws://localhost:2012/");

        _Ws.onmessage = function (evt)
        {
            AppendMessage("# " + ToBinString(evt.data) + "<br />");
        };

        _Ws.onopen = function ()
        {
            AppendMessage("* Connection open.<br />");
        }

        _Ws.onclose = function ()
        {
            AppendMessage('* Connection closed<br/>');
        }
    }

    function ToBinString(array)
    {
        var reader = new window.FileReader();

        reader.readAsText(array);

        reader.onloadend = function ()
        {
            base64data = reader.result;
            //console.log(base64data);
        }

        return base64data;
    }

    function ConnectWebSocket()
    {
        ConnectSocketServer();
    }

    function DisconnectWebSocket()
    {
        if (_Ws) {
            _Ws.close();
        }
    }

    function EraseMessage()
    {
        $('#htmlBody').html('');
    }
</script>
    <body>
        <input type="button" id="connectButton" value="Connect" onclick="ConnectWebSocket()"/> <input type="button" id="DisconnectButton" value="Disconnect" onclick="DisconnectWebSocket()"/> <input type="text" id="messageInput" /> <input type="button" id="sendButton" value="Send" onclick="    sendMessage()"/> <input type="button" id="" value="Erase" onclick="    eraseMessage()" /> <br />
        <div id="introduction">Send the following message and then see the messages received:
            <ul>
                <li>ECHO [ANY TEXT] - "ECHO I love you!"</li>
                <li>ADD [INTEGER A] [INTEGER B] - "ADD 100 150"</li>
                <li>MULT [INTEGER A] [INTEGER B] - "MULT 60 28"</li>
                <li>SUB [INTEGER A] [INTEGER B] - "SUB 100 77"</li>
            </ul>
        </div>
        <div id="htmlBody"></div>
        
    </body>
</asp:Content>
