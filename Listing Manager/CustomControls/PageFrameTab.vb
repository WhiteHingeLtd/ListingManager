Public Class PageFrameTab
    Inherits TabItem

    Public Sub New(Page As Page)
        Header = Page.Title
        Dim Frame as new Frame
        Frame.Content = Page
        me.Content = Frame
        End Sub

End Class
