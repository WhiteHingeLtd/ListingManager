Imports System.ComponentModel
Imports System.Threading
Imports OxyPlot
Imports OxyPlot.Axes
Imports WHLClasses

Class Sales_TimeOfDay
    Inherits ThreadedPage

    Friend Overrides Sub TabClosing(ByRef Cancel As Boolean)
    End Sub
    Friend Overrides Function SupportsMultipleTabs() As Boolean
        Return False
    End Function

    Private Sub TimeOfDayDashboard_Loaded(sender As Object, e As RoutedEventArgs) Handles MyBase.Loaded
        StartDate.SelectedDate = Today.AddDays(-61)
        EndDate.SelectedDate = Today.AddDays(-1)
        StartDate.DisplayDateEnd = Today
        EndDate.DisplayDateEnd = Today
        Reset()
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

    Private Sub Reset()
        TimeOfDayGraph.Visibility = Visibility.Collapsed
        TimeOfDayProgress.Visibility = Visibility.Visible
        CurrentStartDate = StartDate.SelectedDate
        CurrentEndDate = EndDate.SelectedDate
    End Sub

    Dim TimeOfDayModel As PlotModel
    Dim CurrentStartDate As DateTime
    Dim CurrentEndDate As DateTime

    Private Sub BackgroundFlagship()
        Worker.ReportProgress(0, "Preparing Graph...")

        'Create the graph, or "plotmodel"
        TimeOfDayModel = New PlotModel()

        'Do the date axis.
        Dim DateAxis As New TimeSpanAxis
        DateAxis.Position = AxisPosition.Bottom
        DateAxis.Maximum = TimeSpanAxis.ToDouble(New TimeSpan(0, 23, 50, 0))
        DateAxis.AbsoluteMaximum = TimeSpanAxis.ToDouble(New TimeSpan(0, 23, 50, 0))
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

        'OH BOY THE SERIES'
        Dim SundaySeries As New Series.LineSeries
        SundaySeries.Title = "Sunday"
        SundaySeries.CanTrackerInterpolatePoints = False
        SundaySeries.Color = OxyColors.DarkBlue
        Dim MondaySeries As New Series.LineSeries
        MondaySeries.Title = "Monday"
        MondaySeries.CanTrackerInterpolatePoints = False
        MondaySeries.Color = OxyColors.DarkGreen
        Dim TuesdaySeries As New Series.LineSeries
        TuesdaySeries.Title = "Tuesday"
        TuesdaySeries.CanTrackerInterpolatePoints = False
        TuesdaySeries.Color = OxyColors.DarkRed
        Dim WednesdaySeries As New Series.LineSeries
        WednesdaySeries.Title = "Wednesday"
        WednesdaySeries.CanTrackerInterpolatePoints = False
        WednesdaySeries.Color = OxyColors.DarkOrange
        Dim ThursdaySeries As New Series.LineSeries
        ThursdaySeries.Title = "Thursday"
        ThursdaySeries.CanTrackerInterpolatePoints = False
        ThursdaySeries.Color = OxyColors.DarkOrchid
        Dim FridaySeries As New Series.LineSeries
        FridaySeries.Title = "Friday"
        FridaySeries.CanTrackerInterpolatePoints = False
        FridaySeries.Color = OxyColors.DarkGoldenrod
        Dim SaturdaySeries As New Series.LineSeries
        SaturdaySeries.Title = "Saturday"
        SaturdaySeries.CanTrackerInterpolatePoints = False
        SaturdaySeries.Color = OxyColors.DimGray

        Worker.ReportProgress(0, "Getting data...")
        'Now get some datas.
        Dim Query As String = "SELECT DAYOFWEEK(orderDateTime) as Weekday, Count(OrderId) as Orders, Substring(OrderDateTime,12,4), STR_TO_DATE(concat(Substring(OrderDateTime,12,4),'0'),'%H:%i') as TimeBlock FROM (SELECT * FROM whldata.newsales_raw WHERE (orderDateTime>= '" + CurrentStartDate.ToString("yyyy-MM-dd") + " 00:00:00' AND orderDateTime<= '" + CurrentEndDate.ToString("yyyy-MM-dd") + " 23:59:59' AND NOT (orderDateTime is NULL))GROUP BY orderId) as a GROUP BY Substring(OrderDateTime,12,4), DAYOFWEEK(orderDateTime);"
        Dim Data As List(Of Dictionary(Of String, Object)) = MySQL.SelectDataDictionary(Query)
        Worker.ReportProgress(0, "Graphing Data...")
        'Gotta iterate
        Dim MaxYaxisValue As Double = 0
        For Each Line As Dictionary(Of String, Object) In Data
            If Line("Orders") > MaxYaxisValue Then
                MaxYaxisValue = Line("Orders")
            End If

            'Dim AdjustedTime As TimeSpan = New TimeSpan(0, 0, Math.Floor(DirectCast(Line("TimeBlock"), TimeSpan).TotalMinutes / 3) * 3, 0, 0)
            Dim AdjustedTime As TimeSpan = Line("TimeBlock")
            Select Case Line("Weekday")
                Case 1
                    SundaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 2
                    MondaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 3
                    TuesdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 4
                    WednesdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 5
                    ThursdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 6
                    FridaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
                Case 7
                    SaturdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(AdjustedTime), Line("Orders")))
            End Select

        Next
        Dim Times As New List(Of TimeSpan)
        For I = 0 To 143
            Times.Add(New TimeSpan(0, 0, I * 10, 0))
        Next
        'Add the zeroes
        For Each Timeofday As TimeSpan In Times
            If SundaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then SundaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If MondaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then MondaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If TuesdaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then TuesdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If WednesdaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then WednesdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If ThursdaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then ThursdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If FridaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then FridaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
            If SaturdaySeries.Points.Where(Function(X As DataPoint) X.X.Equals(TimeSpanAxis.ToDouble(Timeofday))).Count = 0 Then SaturdaySeries.Points.Add(New DataPoint(TimeSpanAxis.ToDouble(Timeofday), 0))
        Next
        'And order the series for good measure
        SundaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        MondaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        TuesdaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        WednesdaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        ThursdaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        FridaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))
        SaturdaySeries.Points.Sort(Function(X As DataPoint, Y As DataPoint) X.X.CompareTo(Y.X))

        AmountAxis.Maximum = MaxYaxisValue * 1.1
        AmountAxis.AbsoluteMaximum = MaxYaxisValue * 1.1

        'Add everything to the model
        TimeOfDayModel.Series.Add(SundaySeries)
        TimeOfDayModel.Series.Add(MondaySeries)
        TimeOfDayModel.Series.Add(TuesdaySeries)
        TimeOfDayModel.Series.Add(WednesdaySeries)
        TimeOfDayModel.Series.Add(ThursdaySeries)
        TimeOfDayModel.Series.Add(FridaySeries)
        TimeOfDayModel.Series.Add(SaturdaySeries)

        TimeOfDayModel.Axes.Add(DateAxis)
        TimeOfDayModel.Axes.Add(AmountAxis)

    End Sub
    Private Sub ForegroundFlagship()
        UpdateStatus("Showing flagship graph...")
        TimeOfDayGraph.Model = TimeOfDayModel
        TimeOfDayGraph.Visibility = Visibility.Visible
        TimeOfDayGraph.InvalidateVisual()
        TimeOfDayProgress.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BackgroundHighlight1()
        Worker.ReportProgress(0, "Loading time of day graph...")

    End Sub
    Private Sub ForegroundHighlight1()
        UpdateStatus("Showing time of day graph...")
    End Sub
    Private Sub BackgroundHighlight2()
        Worker.ReportProgress(0, "Killing time")
        Thread.Sleep(500)
    End Sub
    Private Sub ForegroundHighlight2()
        UpdateStatus("Ready")
    End Sub




#End Region

    Private Sub RefreshButton_Click(sender As Object, e As RoutedEventArgs) Handles RefreshButton.Click
        Reset()
        StartLoading()
    End Sub

End Class
