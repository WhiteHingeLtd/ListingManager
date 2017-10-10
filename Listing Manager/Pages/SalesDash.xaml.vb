Imports System.ComponentModel
Imports System.Threading
Imports OxyPlot
Imports OxyPlot.Axes
Imports WHLClasses
Imports WHLClasses.MySQL_Old

Class SalesDash

    Friend Sub New(Ref As MainWindow)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        MainWindowRef = Ref
    End Sub

    Friend Overrides Function SupportsMultipleTabs() As Boolean
        Return False
    End Function

    Friend Overrides Sub TabClosing(ByRef Cancel As Boolean)
        'Nah it's fine
    End Sub

    Private Sub ThreadedPage_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateStatus("Loading flagship graph...")
        StartLoading()
    End Sub

#Region "Structured dahboard loading"
    Private Sub StartLoading()
        StartFlagship()
    End Sub

    Private Sub StartFlagship()
        ProcessInBackgroundWithCallback(AddressOf ProcessFlagship, AddressOf FinishFlagship)
    End Sub

    Private Sub ProcessFlagship()
        BackgroundFlagship()
    End Sub

    Private Sub FinishFlagship(Sender As Object, e As RunWorkerCompletedEventArgs)
        ForegroundFlagship()
        StartFirstHighlight()
    End Sub

    Private Sub StartFirstHighlight()
        ProcessInBackgroundWithCallback(AddressOf ProcessFirstHighlight, AddressOf FinishFirstHighlight)
    End Sub

    Private Sub ProcessFirstHighlight()
        BackgroundHighlight1()
    End Sub

    Private Sub FinishFirstHighlight(Sender As Object, e As RunWorkerCompletedEventArgs)
        ForegroundHighlight1()
        StartSecondHighlight()
    End Sub
    Private Sub StartSecondHighlight()
        ProcessInBackgroundWithCallback(AddressOf ProcessSecondHighlight, AddressOf FinishSecondHighlight)
    End Sub

    Private Sub ProcessSecondHighlight()
        BackgroundHighlight2()
    End Sub

    Private Sub FinishSecondHighlight(Sender As Object, e As RunWorkerCompletedEventArgs)
        ForegroundHighlight2()
    End Sub
#End Region

#Region "FillData"

    Dim FlagshipModel As PlotModel
    Dim TimeOfDayModel As PlotModel

    Private Sub BackgroundFlagship()
        'Define out boundaries
        Dim StartDate As Date = Today.AddDays(-61)
        Dim EndDate As Date = Today.AddDays(-1)

        'Create the graph, or "plotmodel"
        FlagshipModel = New PlotModel()
        'Do the date axis.
        Dim DateAxis As New DateTimeAxis
        DateAxis.Position = AxisPosition.Bottom
        DateAxis.Maximum = DateTimeAxis.ToDouble(EndDate)
        DateAxis.AbsoluteMaximum = DateTimeAxis.ToDouble(EndDate)
        DateAxis.Minimum = DateTimeAxis.ToDouble(StartDate)
        DateAxis.AbsoluteMinimum = DateTimeAxis.ToDouble(StartDate)
        DateAxis.Title = "Date"
        DateAxis.StringFormat = "dd MMM"
        DateAxis.MinorIntervalType = DateTimeIntervalType.Days
        'And now do the left axis
        Dim AmountAxis As New LinearAxis
        AmountAxis.Minimum = 0
        AmountAxis.AbsoluteMinimum = 0
        'We can set the maximum value on here when we've actually done it.
        AmountAxis.Position = AxisPosition.Left
        AmountAxis.Title = "Amount"

        'Axis' are done. Gotta make some series now.
        Dim OrdersSeries As New Series.LineSeries
        Dim ProfitSeries As New Series.LineSeries
        Dim TurnoverSeries As New Series.LineSeries

        'Name and property the bastards
        OrdersSeries.Title = "Order Count"
        OrdersSeries.CanTrackerInterpolatePoints = False
        OrdersSeries.Color = OxyColors.LimeGreen
        ProfitSeries.Title = "Grodd Profit"
        ProfitSeries.CanTrackerInterpolatePoints = False
        ProfitSeries.Color = OxyColors.MediumVioletRed
        TurnoverSeries.Title = "Turnover"
        TurnoverSeries.CanTrackerInterpolatePoints = False
        TurnoverSeries.Color = OxyColors.RoyalBlue

        'Now get some datas.
        Dim Data As List(Of Dictionary(Of String, Object)) = MySQL.SelectDataDictionary("SELECT OrderDate, Sum(Retail+PostagePaid) as Turnover, Sum(Total_Profit) as Profit, Count(OrderID) as Orders FROM whldata.newsales_financials WHERE OrderDate>='" + StartDate.ToString("yyyy-MM-dd") + "' AND OrderDate <= '" + EndDate.ToString("yyyy-MM-dd") + "' Group by OrderDate Order By OrderDate Asc;")
        'Gotta iterate
        Dim MaxYaxisValue As Double = 0
        For Each Line As Dictionary(Of String, Object) In Data
            If Line("Turnover") > MaxYaxisValue Then
                MaxYaxisValue = Line("Turnover")
            End If
            OrdersSeries.Points.Add(New DataPoint(DateTimeAxis.ToDouble(Line("OrderDate")), Line("Orders")))
            ProfitSeries.Points.Add(New DataPoint(DateTimeAxis.ToDouble(Line("OrderDate")), Line("Profit")))
            TurnoverSeries.Points.Add(New DataPoint(DateTimeAxis.ToDouble(Line("OrderDate")), Line("Turnover")))
        Next

        AmountAxis.Maximum = MaxYaxisValue * 1.1
        AmountAxis.AbsoluteMaximum = MaxYaxisValue * 1.1

        'Add everything to the model
        FlagshipModel.Series.Add(OrdersSeries)
        FlagshipModel.Series.Add(ProfitSeries)
        FlagshipModel.Series.Add(TurnoverSeries)

        FlagshipModel.Axes.Add(DateAxis)
        FlagshipModel.Axes.Add(AmountAxis)




    End Sub
    Private Sub ForegroundFlagship()
        UpdateStatus("Showing flagship graph...")
        Graph.Model = FlagshipModel
        Graph.Visibility = Visibility.Visible
        FlagshipProgress.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BackgroundHighlight1()
        Worker.ReportProgress(0, "Loading time of day graph...")

        'Time to do it agian!
        Dim StartDate As Date = Today.AddDays(-61)
        Dim EndDate As Date = Today.AddDays(-1)

        'Create the graph, or "plotmodel"
        TimeOfDayModel = New PlotModel()

        'Do the date axis.
        Dim DateAxis As New TimeSpanAxis
        DateAxis.Position = AxisPosition.Bottom
        DateAxis.Maximum = TimeSpanAxis.ToDouble(New TimeSpan(1, 0, 0, 0))
        DateAxis.AbsoluteMaximum = TimeSpanAxis.ToDouble(New TimeSpan(1, 0, 0, 0))
        DateAxis.Minimum = TimeSpanAxis.ToDouble(New TimeSpan(0))
        DateAxis.AbsoluteMinimum = TimeSpanAxis.ToDouble(New TimeSpan(0))
        DateAxis.Title = "Time (24hr)"
        DateAxis.StringFormat = "hh:mm"

        'And now do the left axis
        Dim AmountAxis As New LinearAxis
        AmountAxis.Minimum = 0
        AmountAxis.AbsoluteMinimum = 0
        'We can set the maximum value on here when we've actually done it.
        AmountAxis.Position = AxisPosition.Left
        AmountAxis.Title = "Orders"

        'Axis' are done. Gotta make some series now.
        Dim TimeSeries As New Series.LineSeries

        'Name and property the bastards
        TimeSeries.Title = "Order Count"
        TimeSeries.CanTrackerInterpolatePoints = False
        TimeSeries.Color = OxyColors.DarkOrange
        TimeSeries.RenderInLegend = False
        'Now get some datas.
        Dim Data As List(Of Dictionary(Of String, Object)) = MySQL.SelectDataDictionary("SELECT Count(OrderId) as Orders, Substring(OrderDateTime,12,4), STR_TO_DATE(concat(Substring(OrderDateTime,12,4),'0'),'%H:%i') as TimeBlock FROM (SELECT * FROM whldata.newsales_raw WHERE (orderDate>= '" + StartDate.ToString("yyyy-MM-dd") + "' AND orderDate<= '" + EndDate.ToString("yyyy-MM-dd") + "')GROUP BY orderId) as a GROUP BY Substring(OrderDateTime,12,4);")
        'Gotta iterate
        Dim MaxYaxisValue As Double = 0
        For Each Line As Dictionary(Of String, Object) In Data
            If Line("Orders") > MaxYaxisValue Then
                MaxYaxisValue = Line("Orders")
            End If
            TimeSeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Line("TimeBlock")), Line("Orders")))
        Next

        AmountAxis.Maximum = MaxYaxisValue * 1.1
        AmountAxis.AbsoluteMaximum = MaxYaxisValue * 1.1

        'Add everything to the model
        TimeOfDayModel.Series.Add(TimeSeries)

        TimeOfDayModel.Axes.Add(DateAxis)
        TimeOfDayModel.Axes.Add(AmountAxis)
    End Sub
    Private Sub ForegroundHighlight1()
        UpdateStatus("Showing time of day graph...")
        Highlight1Graph.Model = TimeOfDayModel
        Highlight1Graph.Visibility = Visibility.Visible
        Highlight1Progress.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BackgroundHighlight2()
        Worker.ReportProgress(0, "Killing time")
        Thread.Sleep(500)
    End Sub
    Private Sub ForegroundHighlight2()
        Highlight2Progress.Visibility = Visibility.Collapsed
        UpdateStatus("Ready")
    End Sub

    Private Sub TiemofDayHighlightTitle_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles TiemofDayHighlightTitle.MouseLeftButtonUp
        MainWindowRef.NewTab(New Sales_TimeOfDay)
    End Sub

#End Region




End Class
