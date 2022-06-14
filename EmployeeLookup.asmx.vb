Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Collections.Generic

Imports Devart.Data.Oracle







' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class EmployeeLookup
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Function lookupEmployeeByName(ByVal prefixText As String) As String()

        Dim ds As System.Data.DataSet = Nothing
        Dim mySql As String

        Dim count As Integer = 10
        Dim items As New List(Of String)(count)

        mySql = ""
        mySql += "SELECT  UPPER(TRIM(E.lastname)) || ', ' || UPPER(TRIM(E.firstname)) || '   (' || TRIM(S.sitename) || ')   Username:' || E.domain || '\' || UPPER(TRIM(E.username)) NAME"
        mySql += " From refemployee E,refsite S"
        mySql += " WHERE 1=1 AND E.plantcode = S.rcfaflid AND E.inactive_flag = 'N'  AND E.domain = 'NA'"
        mySql += " AND UPPER(E.lastname) LIKE '" + Trim(UCase(prefixText)) + "%'"
        mySql += " AND rownum <=20"
        mySql += " ORDER BY E.lastname"

        ds = GetOracleDataSet(mySql, "")

        If ds IsNot Nothing Then
            'Dim row As DataRow
            'row = ds.Tables(0).NewRow
            'row("PROCESS") = "JAMES"
            'ds.Tables(0).Rows.Add(row)

        End If


        For Each dr As DataRow In ds.Tables(0).Rows

            items.Add(dr("NAME").ToString())

        Next

        Return items.ToArray()

        If ds IsNot Nothing Then
            ds.Dispose()
            ds = Nothing
        End If



    End Function


    Shared Function GetOracleDataSet(ByVal sql As String, Optional ByVal connection As String = "", Optional ByVal provider As String = "") As DataSet
        Dim conCust As OracleConnection = Nothing
        Dim cmdSql As OracleCommand = Nothing
        Dim dbPF As DbProviderFactory = Nothing
        Dim ds As New DataSet
        Dim myDataAdapter As New OracleDataAdapter()

        'Dim rowid As String = ""
        Try
            If connection.Length = 0 Then
                connection = ConfigurationManager.ConnectionStrings.Item("connectionRCFATST").ConnectionString
            End If
            'If provider.Length = 0 Then
            '    provider = ConfigurationManager.ConnectionStrings.Item("connectionRCFATST").ProviderName
            'End If


            'dbPF = DbProviderFactories.GetFactory(provider)
            'conCust = CType(dbPF.CreateConnection, OracleConnection)
            'conCust.ConnectionString = connection
            conCust = New OracleConnection(connection)
            conCust.Open()
            ds.EnforceConstraints = False
            cmdSql = New OracleCommand(sql, conCust) ' CType(dbPF.CreateCommand, OracleCommand)
            'cmdSql.Connection = conCust
            'cmdSql.CommandText = sql
            myDataAdapter = New OracleDataAdapter(cmdSql)
            ds.Tables.Add("ResultTable")
            ds.Tables("ResultTable").BeginLoadData()
            myDataAdapter.Fill(ds.Tables("ResultTable"))
            ds.Tables("ResultTable").EndLoadData()

        Catch ex As Exception
            ds = Nothing
            'Return Nothing
            Throw 'ApplicationException("GetOracleDataSet - " & sql, ex)
        Finally
            GetOracleDataSet = ds
            conCust.Close()
            If Not conCust Is Nothing Then conCust = Nothing
            If Not cmdSql Is Nothing Then cmdSql = Nothing
            If Not dbPF Is Nothing Then dbPF = Nothing
            If Not myDataAdapter Is Nothing Then myDataAdapter = Nothing
            If Not ds Is Nothing Then ds = Nothing
        End Try
    End Function


End Class