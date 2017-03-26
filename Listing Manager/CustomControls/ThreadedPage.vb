Imports System.ComponentModel
Imports System.Windows.Threading

Public Class ThreadedPage
    Inherits Page

    dim Timer as DispatcherTimer
    dim Worker as New BackgroundWorker
    dim Status as textblock
    
    Friend Sub New()
        
        ' Add any initialization after the InitializeComponent() call.
        'Timer = new Dispatchertimer(DispatcherPriority.Background, Me.Dispatcher, New Timespan(0,0,1))

        
    End Sub
    
    Public Sub TimerTick()

    End Sub

    Friend Sub UpdateStatus(NewStatus as string)
        If Status is nothing then Status=Me.Template.FindName("StatusBlock", Me)
        
        Status.Text = NewStatus
        'StatusText..text = NewStatus
    End Sub

End Class

