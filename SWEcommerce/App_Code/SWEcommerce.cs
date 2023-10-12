using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.Services;
//using System.Data.OracleClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;




[WebService(Namespace = "http://Ecommerce_Coban/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class productos
{
    public string estilo { get; set; }
    public string curva { get; set; }
    public string talla { get; set; }
    public string codigo { get; set; }
    public string tipo_producto { get; set; }
    public string descripcion { get; set; }
    public string cantidad { get; set; }
    public string precio { get; set; }
    public string estiloax { get; set; }

    public productos()
    {
        estilo = "";
        curva = "";
        talla = "";
        codigo = "";
        tipo_producto = "";
        descripcion = "";
        cantidad = "";
        precio = "";
        estiloax = "";

    }
}


public class SWEcommerce : System.Web.Services.WebService
{
    public SWEcommerce()
    {


    }


    [WebMethod]
    public List<productos> GetInventario(string codigos)
    {

        List<productos> lista = new List<productos>();
        lista = MetodoSplit(codigos);

        for (int i = 0; i <= lista.Count; i++)
        {
            return lista;
        }
        return lista;
    }


    public List<productos> MetodoSplit(string codigos)
    {


        char delimitador = '|';
        string[] valores = codigos.Split(delimitador);

        List<productos> resultado = new List<productos>();

        OracleConnection conection = new OracleConnection("DATA SOURCE = 172.20.5.32:1521/XE; PASSWORD=ZAPATERIAS; USER ID = ZAPATERIAS;");
        conection.Open();


        for (int i = 0; i <= valores.Length - 1; i++)
        {
            //OracleConnection conection = new OracleConnection("DATA SOURCE = XE; PASSWORD=PORTALES; USER ID = PORTALES;");
            //conection.Open();
            OracleCommand comando = new OracleCommand("SELECT * FROM SW_INVENTARIO WHERE ESTILO = :estilo", conection);
            comando.Parameters.Add(":estilo", valores[i]);
            OracleDataReader lector = comando.ExecuteReader();


            while (lector.Read())
            {

                productos registro = new productos();
                registro.estilo = lector.GetOracleString(0).ToString();
                registro.curva = lector.GetOracleString(1).ToString();
                registro.talla = lector.GetOracleString(2).ToString();
                registro.codigo = lector.GetOracleString(3).ToString();
                registro.tipo_producto = lector.GetOracleString(4).ToString();
                registro.descripcion = lector.GetOracleString(5).ToString();
                //registro.cantidad = lector.GetOracleNumber(6).ToString();
                registro.cantidad = lector.GetOracleDecimal(6).ToString();
                //registro.precio = lector.GetOracleNumber(7).ToString();
                registro.precio = lector.GetOracleDecimal(7).ToString();
                registro.estiloax = lector.GetOracleString(8).ToString();
                resultado.Add(registro);
            }

        }

        conection.Close();

        return resultado;

    }


    [WebMethod]
    public string SetPedidos(string cliente, string pedido, string fpago)
    {
        bool insertDatos = SplitObject(cliente, pedido, fpago);


        string resPedido;

        if (insertDatos == true)
        {
            resPedido = "PEDIDO INSERTADO CORRECTAMENTE";
        }
        else
        {
            resPedido = "OCURRIO UN ERRROR AL INSERTAR EL PEDIDO";
        }
        return resPedido;
    }

    public bool SplitObject(string socio, string pedido, string pago)
    {
        char delimitador = '|';
        string[] vSocio = socio.Split(delimitador);
        string[] vPedido = pedido.Split(delimitador);
        string[] vPago = pago.Split(delimitador);

        bool respuestaSocio = InsertSocio(vSocio);
        bool respuestaPedido = InsertPedido(vSocio, vPedido, vPago);


        return respuestaPedido;
    }

    public bool InsertSocio(string[] vSocio)
    {

        string cadConnection = "DATA SOURCE = 172.20.5.32:1521/XE; PASSWORD=ZAPATERIAS; USER ID = ZAPATERIAS;";

        using (var con = new OracleConnection(cadConnection))
        {
            try
            {
                //se abre la conexion
                con.Open();
                using (var comando = new OracleCommand())
                {
                    //al comando se le asigna la conexion
                    comando.Connection = con;

                    // se le indica el tipo de comando y nombre 
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.CommandText = "WMS_P_CREAR_SOCIO_PW";
                    //comando.BindByName = true;
                    //Agregamos los parametros a enviar al procedimiento
                    comando.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = vSocio[0];
                    //comando.Parameters.Add("p_nombre_cliente", OracleDbType.Varchar2).Value = vSocio[0];
                    comando.Parameters.Add("p_telefono", OracleDbType.Varchar2).Value = vSocio[1];
                    comando.Parameters.Add("p_nit", OracleDbType.Varchar2).Value = vSocio[2];
                    comando.Parameters.Add("p_direccion1", OracleDbType.Varchar2).Value = vSocio[3];
                    comando.Parameters.Add("p_email", OracleDbType.Varchar2).Value = vSocio[4];
                    comando.Parameters.Add("p_cliente_pedido", OracleDbType.Varchar2).Value = vSocio[5];
                    comando.Parameters.Add("p_direccion_envio", OracleDbType.Varchar2).Value = vSocio[6];
                    comando.Parameters.Add("p_horario_entrega", OracleDbType.Varchar2).Value = vSocio[7];

                    comando.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        return true;
    }

    public bool InsertPedido(string[] vSocio, string[] vPedido, string[] vPago)
    {
        string cadConnection = "DATA SOURCE = 172.20.5.32:1521/XE; PASSWORD=ZAPATERIAS; USER ID = ZAPATERIAS;";

        Console.Write(vSocio);
        Console.Write(vPedido);
        Console.Write(vPago);

        using (var con = new OracleConnection(cadConnection))
        {
            try
            {
                //se abre la conexion
                con.Open();
                using (var comando = new OracleCommand())
                {
                    //al comando se le asigna la conexion
                    comando.Connection = con;


                    // se le indica el tipo de comando y nombre 
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.CommandText = "WMS_P_CREAR_PEDIDO_PW";
                    //comando.BindByName = true;
                    //Agregamos los parametros a enviar al procedimiento
                    comando.Parameters.Add("P_SOCIO", OracleDbType.Varchar2).Value = vSocio[2];
                    comando.Parameters.Add("P_LEGACY", OracleDbType.Varchar2).Value = vPedido[0];
                    comando.Parameters.Add("P_CANTIDAD", OracleDbType.Double).Value = vPedido[1];
                    comando.Parameters.Add("P_ENVIO", OracleDbType.Varchar2).Value = vPedido[2];

                    /*if (vPago[0].Equals("1"))
                    {
                        comando.Parameters.Add("P_PAGO", OracleDbType.Varchar2).Value = string.Format("Foma de pago: {0} Tipo Tarjeta: {1} No.Tarjeta: {2} Vencimiento: {3} Autorizacion: {4} Transaccion: {5}"
                                                                                                    , vPago[1], vPago[2], vPago[3], vPago[4], vPago[5], vPago[6]);
                    }
                    else if (vPago[0].Equals("2"))
                    {
                        comando.Parameters.Add("P_PAGO", OracleDbType.Varchar2).Value = string.Format("Foma de pago: {0} ", vPago[1]);  

                    }*/

                    comando.Parameters.Add("P_CUPONOKI", OracleDbType.Varchar2).Value = vPedido[3];
                    comando.Parameters.Add("P_MONTOOKI", OracleDbType.Double).Value = vPedido[4];
                    comando.Parameters.Add("P_CUPONCOBAN", OracleDbType.Varchar2).Value = vPedido[5];
                    comando.Parameters.Add("P_PORCENTAJEDESC", OracleDbType.Double).Value = vPedido[6];


                    comando.ExecuteNonQuery();

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        return true;
    }

}