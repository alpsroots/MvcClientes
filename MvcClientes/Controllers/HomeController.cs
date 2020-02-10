using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcClientes.Models;

namespace MvcClientes.Controllers
{
    public class HomeController : Controller
    {
        private AppSettings AppSettings { get; set; }

        public HomeController(IOptions<AppSettings> settings)
        {
            AppSettings = settings.Value;
        }

        public ActionResult Index()
        {
            List<Cliente> clientes = new List<Cliente>();
            string constr = AppSettings.SqlConexao;
            string query = "SELECT ClienteId, Nome, Endereco, Email FROM Clientes order by ClienteId";
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            clientes.Add(new Cliente
                            {
                                ClienteId = Convert.ToInt32(sdr["ClienteId"]),
                                Nome = Convert.ToString(sdr["Nome"]),
                                Endereco = Convert.ToString(sdr["Endereco"]),
                                Email = Convert.ToString(sdr["Email"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (clientes.Count == 0)
            {
                clientes.Add(new Cliente());
            }
            return View(clientes);
        }
        
        [HttpPost]
        public ActionResult InsertCliente([FromBody] Cliente cliente)
        {
            string query = "INSERT INTO Clientes VALUES(@Nome, @Endereco, @Email)";
            query += "SELECT SCOPE_IDENTITY()";
            string constr = AppSettings.SqlConexao;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                    cmd.Parameters.AddWithValue("@Endereco", cliente.Endereco);
                    cmd.Parameters.AddWithValue("@Email", cliente.Email);
                    cmd.Connection = con;
                    con.Open();
                    cliente.ClienteId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                }
            }
            return Json(cliente);
        }
        
        [HttpPost]
        public IActionResult UpdateCliente([FromBody] Cliente cliente)
        {
            string query = "UPDATE Clientes SET Nome=@Nome, Endereco=@Endereco, Email=@Email WHERE ClienteId = @ClienteId";
            string constr = AppSettings.SqlConexao;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", cliente.ClienteId);
                    cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                    cmd.Parameters.AddWithValue("@Endereco", cliente.Endereco);
                    cmd.Parameters.AddWithValue("@Email", cliente.Email);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return new EmptyResult();
        }
        
        [HttpPost]
        public IActionResult DeleteCliente([FromBody] int clienteid)
        {
            string query = "DELETE FROM Clientes WHERE ClienteId=@ClienteId";
            string constr = AppSettings.SqlConexao;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", clienteid);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return new EmptyResult();
        }
    }
}
