Imports System.Windows.Controls.Ribbon
Imports Microsoft.Windows.Shell
Imports WHLClasses
Imports WHLClasses.Authentication
Imports WHLClasses.Linnworks.Auth

Class MainWindow
    Inherits RibbonWindow

#Region "Vars"
    'Data Object Collections
    Friend Data_Skus As SkuCollection
    Friend Data_Employees as EmployeeCollection

    'User related objects
    Friend User_Employee as Authclass
#End Region

#Region "Startup and runtime"

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        UpdateShellColor()
        AddHandler SystemParameters.StaticPropertyChanged, AddressOf UpdateShellColor
    End Sub

    Private Sub UpdateShellColor()
        MainRibbon.Background = New SolidColorBrush(Color.FromRgb((SystemParameters.WindowGlassColor.R + 510) / 3, (SystemParameters.WindowGlassColor.G + 510) / 3, (SystemParameters.WindowGlassColor.B + 510) / 3))
    End Sub
    
    Private Sub RibbonWindow_Initialized(sender As Object, e As EventArgs) Handles RibbonWindow.Initialized
        dim Splash as new Splash
        Splash.HomeRef = me
        Splash.ShowDialog()
    End Sub

#End Region

#Region "MainZone"
    Friend Sub NewTab(Control As Object)
        Dim Tab As New PageFrameTab(Control)
        MainWindowTabControl.Items.Add(Tab)
        Tab.Focus()
        MainWindowTabControl.SelectedItem = Tab

    End Sub
#End Region

#Region "App Menu"

    Private Sub AppMenu_LoadData_ReloadSkusButton_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub Views_DashboardButton_Click(sender As Object, e As RoutedEventArgs)
        NewTab(New Test)
    End Sub

    Private Sub Views_SalesDashboardButton_Click(sender As Object, e As RoutedEventArgs) Handles Views_SalesDashboardButton.Click
        NewTab(New SalesDash(Me))
    End Sub


#End Region

#Region "Ribbon"


#End Region
End Class
