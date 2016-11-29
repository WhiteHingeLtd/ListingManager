<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="ListingManager._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Listing Manager Lite</h1>
        <p class="lead">Coming soon.</p>
        <p><a href="#" class="btn btn-primary btn-lg">... &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>
                <asp:Label ID="SessionTest" runat="server" Text="Label"></asp:Label></h2>
            <p>
                The main UI won't look anything like this.
            </p>
            <p>
                <a class="btn btn-default" href="#">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Column 2</h2>
            <p>
                I haven&#39;t decided what to put here yet.</p>
            <p>
                <a class="btn btn-default" href="#">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Column 3</h2>
            <p>
                Same with this one.</p>
            <p>
                <a class="btn btn-default" href="#">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
