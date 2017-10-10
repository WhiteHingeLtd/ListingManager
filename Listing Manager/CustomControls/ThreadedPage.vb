Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Threading
Imports System.Windows.Forms

Public MustInherit Class ThreadedPage
    Inherits Page

    Friend Overridable Function SupportsMultipleTabs() As Boolean
        Return True
    End Function

    Friend Timer As DispatcherTimer
    Friend Worker As BackgroundWorker
    Friend MainWindowRef As MainWindow

    Private _Status As TextBlock
    Private _ClockBlock As TextBlock

    Friend Sub New()

        ' Add any initialization after the InitializeComponent() call.
        Timer = New DispatcherTimer(New TimeSpan(0, 0, 0, 1), DispatcherPriority.Normal, AddressOf TimerTick, Dispatcher)
        SetUpWorker()
    End Sub

    Friend Sub SetMainWindowRef(Main As MainWindow)
        MainWindowRef = Main
    End Sub

    Private Sub SetUpWorker()
        Worker = New BackgroundWorker()
        Worker.WorkerReportsProgress = True
        AddHandler Worker.DoWork, AddressOf WorkerHandler
        AddHandler Worker.ProgressChanged, AddressOf WorkerProgress
        AddHandler Worker.RunWorkerCompleted, AddressOf WorkerComplete
    End Sub

    Public Sub TimerTick(Sender As Object, E As EventArgs)
        If _ClockBlock Is Nothing Then
            _ClockBlock = TryCast(Template.FindName("ClockBlock", Me), TextBlock)
        Else
            _ClockBlock.Text = DateTime.Now.ToString("HH:mm:ss")
        End If

    End Sub

    Friend Sub UpdateStatus(NewStatus As String)
        If _Status Is Nothing Then
            _Status = TryCast(Template.FindName("StatusBlock", Me), TextBlock)
        End If

        _Status.Text = NewStatus
        'StatusText..text = NewStatus
    End Sub


    Private _WorkerRunning As Boolean = False
    Friend Sub ProcessInBackgroundModally(Process As Action)
        _WorkerRunning = False
        Worker.RunWorkerAsync(Process)
        While Worker.IsBusy Or _WorkerRunning
            Forms.Application.DoEvents()
            Thread.Sleep(16)
        End While
    End Sub

    Friend Sub ProcessInBackgroundWithCallback(Process As Action, Callback As RunWorkerCompletedEventHandler)
        If Not _WorkerRunning Then
            _WorkerRunning = False
            BackgroundWorkerCallback = Callback
            Worker.RunWorkerAsync(Process)
        Else
            Throw New InvalidOperationException("The background worker for this tab is busy.")
        End If

    End Sub

    Dim BackgroundWorkerCallback As RunWorkerCompletedEventHandler

    Private Sub WorkerHandler(Sender As Object, E As DoWorkEventArgs)
        DirectCast(E.Argument, Action).Invoke()
        _WorkerRunning = False
    End Sub

    Private Sub WorkerComplete(Sender As Object, E As RunWorkerCompletedEventArgs)
        BackgroundWorkerCallback.Invoke(Sender, E)
    End Sub

    Friend Overridable Sub WorkerProgress(Sender As Object, E As ProgressChangedEventArgs)
        UpdateStatus(E.UserState.ToString())
    End Sub

    Friend MustOverride Sub TabClosing(ByRef Cancel As Boolean)

End Class

