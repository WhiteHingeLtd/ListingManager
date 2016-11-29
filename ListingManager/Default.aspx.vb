Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'SessionTest.Text = CType(Session("User"), WHLClasses.Employee).FullName
    End Sub
End Class