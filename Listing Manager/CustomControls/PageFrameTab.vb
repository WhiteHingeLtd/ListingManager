Public Class PageFrameTab
    Inherits TabItem

    Public Sub New(Page As Page)
        Header = Page.Title
        Dim Frame As New Frame
        Frame.Content = Page
        Me.Content = Frame

    End Sub



End Class
