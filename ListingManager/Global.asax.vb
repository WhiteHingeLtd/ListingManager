Imports System.Web.Optimization
Imports WHLClasses

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        RouteConfig.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)

        Application.Add("EmpCol", New EmployeeCollection)
    End Sub
End Class