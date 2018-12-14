using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WFARTHA.Entities;
using WFARTHA.Models;
using WFARTHA.Services;

namespace WFARTHA.Controllers
{
    [Authorize]
    public class FlujosController : Controller
    {
        private WFARTHAEntities db = new WFARTHAEntities();

        [HttpPost]
        public string Procesa2(decimal id)
        {
            String res = "Archivo generado";
            ProcesaFlujo pf = new ProcesaFlujo();
            if (ModelState.IsValid)
            {

                res = pf.procesa(id);
                if (res.Equals("0"))//Aprobado
                {
                    //return RedirectToAction("Details", "Solicitudes", new { id = id });
                    res = "Archivo generado";
                }
                //else if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
                //{

                //    return RedirectToAction("Details", "Solicitudes", new { id = id });
                //}
                //else
                //{
                //    TempData["error"] = res;
                //    return RedirectToAction("Details", "Solicitudes", new { id = id });
                //}


            }

            return res;
        }

        //LEJGG 12-12-2018
        public string procesacoc(decimal num_doc)//MGC 29-10-2018 Configuración de estatus
        {
            bool nextSol = false;////MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario
            string correcto = String.Empty;
            WFARTHAEntities db = new WFARTHAEntities();
            FLUJO actual = new FLUJO();
            string recurrente = "";
            bool emails = false; //MGC 08-10-2018 Obtener los datos para el correo
            string emailsto = ""; //MGC 09-10-2018 Envío de correos
            if (true)//---------------------------NUEVO REGISTRO
            {
                DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);

                //actual = db.FLUJOes.Where(fl => fl.NUM_DOC == num_doc).FirstOrDefault();//MGC 05-10-2018 Modificación para work flow al ser editada
                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).OrderByDescending(x => x.POS).FirstOrDefault();//MGC 05-10-2018 Modificación para work flow al ser editada


                //MGC 08-10-2018 Obtener los datos para el correo
                WORKFV wf = db.WORKFHs.Where(a => a.ID == actual.WORKF_ID).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();

                WORKFP wp = wf.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).OrderBy(a => a.POS).FirstOrDefault();
                string email = ""; //MGC 08-10-2018 Obtener el nombre del cliente
                email = wp.EMAIL; //MGC 08-10-2018 Obtener el nombre del cliente

                if (email == "X")
                {
                    emails = true;
                }

                int step = 0;
                if (actual.STEP_AUTO == 0)
                {
                    //step = Convert.ToInt32(actual.STEP_AUTO) + 1;                      
                    step = Convert.ToInt32(actual.STEP_AUTO);//MGC 19-10-2018 Cambio a detonador  
                }

                List<DET_AGENTECA> dap = db.DET_AGENTECA.Where(a => a.VERSION == actual.RUTA_VERSION && a.ID_RUTA_AGENTE == actual.ID_RUTA_A && a.STEP_FASE == step).OrderByDescending(a => a.VERSION).ToList();
                DET_AGENTECA dah = new DET_AGENTECA();
                dah = detAgenteLimite(dap, Convert.ToDecimal(d.MONTO_DOC_MD), step, actual);//MGC 19-10-2018 Cambio a detonador  


                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                int next_step_a = 0;
                if (paso_a.NEXT_STEP != null)
                    next_step_a = (int)paso_a.NEXT_STEP;

                WORKFP next = new WORKFP();
                if (recurrente != "X")
                {
                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                }
                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                {
                    d.ESTATUS_WF = "A";
                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "2";
                    }
                }
                else
                {
                    //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                    FLUJO nuevo = new FLUJO();
                    nuevo.WORKF_ID = next.ID;
                    nuevo.WF_VERSION = next.VERSION;
                    nuevo.WF_POS = next.POS;
                    nuevo.NUM_DOC = actual.NUM_DOC;
                    nuevo.POS = actual.POS + 1;

                    //Agregar autorización MGC
                    nuevo.ID_RUTA_A = actual.ID_RUTA_A;
                    nuevo.RUTA_VERSION = actual.RUTA_VERSION;

                    //MGC 11-12-2018 Agregar Contabilizador 0----------------->
                    nuevo.VERSIONC1 = actual.VERSIONC1;
                    nuevo.VERSIONC2 = actual.VERSIONC2;
                    //MGC 11-12-2018 Agregar Contabilizador 0-----------------<

                    if (next.ACCION.TIPO == "E")
                    {
                        nuevo.USUARIOA_ID = null;
                        nuevo.DETPOS = 0;
                        nuevo.DETVER = 0;
                    }
                    else
                    {
                        if (recurrente != "X")
                        {
                            //MGC 12-11-2018 Se guarda con normalidad
                            FLUJO detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah, step, actual);//MGC 19-10-2018 Cambio a detonador 
                            nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                            nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;
                            nuevo.STEP_AUTO = detA.STEP_AUTO;

                            //MGC 12-11-2018 Se obtiene el back up
                            DateTime fecha = DateTime.Now.Date;
                            DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                            if (del != null)
                                nuevo.USUARIOA_ID = del.USUARIOD_ID;
                            else
                                nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                            nuevo.DETPOS = detA.DETPOS;
                            nuevo.DETVER = dah.VERSION;

                            //MGC 09-10-2018 Envío de correos
                            if (emails)
                            {
                                emails = true;

                                //MGC 09-10-2018 Envío de correos
                                //Obtener el email del creador
                                string emailc = "";
                                emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                                emailsto = emailc;
                                emailc = "";

                                //Obtener el usuario aprobador
                                emailc = db.USUARIOs.Where(us => us.ID == nuevo.USUARIOA_ID).FirstOrDefault().EMAIL;
                                emailsto += "," + emailc;

                                //Obtener el usuario del siguiente aprobador 
                            }
                        }
                        else
                        {
                            nuevo.USUARIOA_ID = null;
                            nuevo.DETPOS = 0;
                            nuevo.DETVER = 0;
                        }
                    }
                    nuevo.ESTATUS = "P";
                    nuevo.FECHAC = DateTime.Now;
                    nuevo.FECHAM = DateTime.Now;
                    db.FLUJOes.Add(nuevo);
                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual

                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "1";
                    }

                    d.ESTATUS_WF = "P";
                    d.ESTATUS = "F";
                    db.Entry(d).State = EntityState.Modified;
                    db.SaveChanges();//MGC 03-12-2018 Loop para firmas y obtener el más actual

                    DOCUMENTO dmod = db.DOCUMENTOes.Find(num_doc);
                    dmod.ESTATUS_WF = "P";
                    dmod.ESTATUS = "F";
                    dmod.ESTATUS_SAP = null;
                    dmod.ESTATUS_PRE = "G";
                    db.Entry(dmod).State = EntityState.Modified;
                    db.SaveChanges();

                    //MGC 03-12-2018 Loop para firmas y obtener el más actual
                    //Actualizar el actual
                    actual.FECHAM = DateTime.Now;
                    db.Entry(actual).State = EntityState.Modified;
                    db.SaveChanges();

                    //MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario-------------------------------------------------->
                    if (d.USUARIOC_ID == d.USUARIOD_ID)
                    {
                        nextSol = true;
                    }
                    //MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario--------------------------------------------------<

                    //MGC 30-10-2018 Agregar mensaje a log de modificación
                    try
                    {
                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                        dl.NUM_DOC = d.NUM_DOC;
                        dl.TYPE_LINE = "M";
                        dl.TYPE = "S";
                        dl.MESSAGE = "Comienza el Proceso de Aprobación";
                        dl.FECHA = DateTime.Now;

                        //db.DOCUMENTOLOGs.Add(dl);//MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario
                        //db.SaveChanges();//MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario
                    }
                    catch (Exception e)
                    {

                    }
                    //MGC 30-10-2018 Agregar mensaje a log de modificación

                    //MGC 16-10-2018 Eliminar msg
                    deleteMesg(d.NUM_DOC);
                }
            }

            //MGC 08-10-2018 Obtener los datos para el correo
            if (emails)
            {
                //Obtener el directorio desde la configuración


                //MGC 08-10-2018 Obtener los datos para el correo comentar provisional
                Email em = new Email();
                string UrlDirectory = getURLPortal();
                //string image = Server.MapPath("~/images/artha_logo.jpg");
                string image = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string page = "Index";
                try
                {
                    //MGC 11-10-2018 No enviar correos 
                    em.enviaMailC(num_doc, true, "ES", UrlDirectory, page, image, emailsto);
                }
                catch (Exception)
                {

                }

            }

            //MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario-------------------------------------------------->
            //MGC 21-11-2018 correr el siguiente flujo cuando el creador y solicitante son el mismo usuario
            if (nextSol)
            {
                //Obtener el documento
                DOCUMENTO _doc = db.DOCUMENTOes.Find(num_doc);


                //Obtener el último flujo registrado
                FLUJO fa = new FLUJO();
                fa = db.FLUJOes.Where(a => a.NUM_DOC.Equals(_doc.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

                //Obtener las acciones
                FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(_doc.NUM_DOC) & a.ESTATUS.Equals("P")).FirstOrDefault();

                //Hacer las modificaciones necesarias el flujo para auto-aprobar
                if (_doc.ESTATUS_C != "C")
                {
                    if (f != null)
                    {
                        string accion = "";
                        accion = db.WORKFPs.Where(a => a.ID.Equals(f.WORKF_ID) & a.POS.Equals(f.WF_POS) & a.VERSION.Equals(f.WF_VERSION)).FirstOrDefault().ACCION.TIPO;

                        if (accion == "A")
                        {

                            //Modificación del flujo, simular 
                            fa.ESTATUS = "A";
                            fa.COMENTARIO = "Auto Aprobado";

                            Flujosprocesa(fa, _doc.USUARIOC_ID);
                        }
                    }
                }

            }
            //MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario--------------------------------------------------<

            return correcto;
        }

        public string getURLPortal()
        {
            WFARTHAEntities db = new WFARTHAEntities();
            string url = "";

            try
            {
                url = db.APPSETTINGs.Where(app => app.NOMBRE == "urlPortal" && app.ACTIVO == true).FirstOrDefault().VALUE.ToString();
            }
            catch (Exception e)
            {

            }

            if (url == null | url == "")
            {
                url = "http://localhost:60621/Correos/Index/";
            }

            return url;
        }

        public void deleteMesg(decimal numdoc)
        {
            WFARTHAEntities db = new WFARTHAEntities();
            //Eliminar los mensajes de la tabla 
            try
            {
                db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(d => d.NUM_DOC == numdoc));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        //Obtener el agente que se ajusta de a partir del monto del documento
        public DET_AGENTECA detAgenteLimite(List<DET_AGENTECA> lag, decimal monto, int step, FLUJO flujo)//MGC 19-10-2018 Cambio a detonador  
        {

            List<DET_AGENTECA> lr = new List<DET_AGENTECA>();
            //MGC 19-10-2018 Cambio a detonador 
            if (lag.Count == 0 & step == 0)
            {
                WFARTHAEntities db = new WFARTHAEntities();
                //Obtener el usuario al que se le creo la orden
                DET_AGENTECC dc = new DET_AGENTECC();

                dc = db.DET_AGENTECC.Where(dtc => dtc.VERSION == flujo.RUTA_VERSION && dtc.USUARIOC_ID == flujo.USUARIOA_ID && dtc.ID_RUTA_AGENTE == flujo.ID_RUTA_A).FirstOrDefault();

                DET_AGENTECA dt = new DET_AGENTECA();

                dt.ID_RUTA_AGENTE = flujo.ID_RUTA_A;
                dt.VERSION = Convert.ToInt32(flujo.RUTA_VERSION);
                dt.STEP_FASE = 0;
                dt.STEP_ACCION = 0;
                dt.LIM_SUP = Convert.ToDecimal(99999999999.00);
                dt.AGENTE_SIG = dc.USUARIOA_ID;

                lr.Add(dt);

            }
            //MGC 19-10-2018 Cambio a detonador 
            else
            {
                //Funcionamiento en cadena
                foreach (DET_AGENTECA ag in lag)
                {
                    if (monto < ag.LIM_SUP)
                    {
                        lr.Add(ag);
                    }
                }
            }

            //Se obtuvieron los registros que pueden validar el monto
            DET_AGENTECA rv = new DET_AGENTECA();
            rv = lr.OrderBy(ll => ll.LIM_SUP).FirstOrDefault();
            return rv;
        }

        public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTECA dah, int step, FLUJO actual)//MGC 19-10-2018 Cambio a detonador 
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

            if (pos.Equals(0))
            {
                List<DET_AGENTECA> dal = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP_FASE == dah.STEP_FASE).OrderByDescending(a => a.VERSION).ToList();

                dap = detAgenteLimite(dal, Convert.ToDecimal(d.MONTO_DOC_MD), step, actual);//MGC 19-10-2018 Cambio a detonador 


            }

            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;
            if (!fin)
            {
                //agente = dap.USUARIOA_ID;
                agente = dap.AGENTE_SIG;
                //f.DETPOS = dap.POS;
                f.DETPOS = dap.STEP_FASE;
                f.STEP_AUTO = dap.STEP_FASE;
            }
            f.USUARIOA_ID = agente;
            return f;
        }

        public void Flujosprocesa(FLUJO f, string user)
        {
            WFARTHAEntities db = new WFARTHAEntities();

            ProcesaFlujo pf = new ProcesaFlujo();
            DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);
            FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

            FLUJO flujo = actual;
            flujo.ESTATUS = f.ESTATUS;
            flujo.FECHAM = DateTime.Now;
            flujo.COMENTARIO = f.COMENTARIO;
            flujo.USUARIOA_ID = user;// User.Identity.Name;//MGC 21-11-2018 Validar si el creador y el solicitante es el mismo usuario

            flujo.ID_RUTA_A = f.ID_RUTA_A;
            flujo.RUTA_VERSION = f.RUTA_VERSION;
            flujo.STEP_AUTO = f.STEP_AUTO;
            string res = pf.procesa(flujo, "", false, "", "");
            if (res.Equals("0"))//Aprobado
            {
                //
            }
            else if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
            {
               //
            }
            else
            {
                //
            }
        }
        //LEJGG 12-12-2018

        [HttpPost]
        public ActionResult Procesa(FLUJO f)
        {

            ProcesaFlujo pf = new ProcesaFlujo();
            DOCUMENTO d = db.DOCUMENTOes.Find(f.NUM_DOC);
            FLUJO actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

            //MGC 12092018
            //List<TS_FORM> tts = db.TS_FORM.Where(a => a.BUKRS_ID.Equals(d.SOCIEDAD_ID) & a.LAND_ID.Equals(d.PAIS_ID)).ToList();

            //bool c = false;
            //if (actual.WORKFP.ACCION.TIPO == "R")
            //{
            //    List<DOCUMENTOT> ddt = new List<DOCUMENTOT>();
            //    foreach (TS_FORM ts in tts)
            //    {
            //        DOCUMENTOT dts = new DOCUMENTOT();
            //        dts.NUM_DOC = f.NUM_DOC;
            //        dts.TSFORM_ID = ts.POS;
            //        try
            //        {
            //            string temp = Request.Form["chk-" + ts.POS].ToString();
            //            if (temp == "on")
            //                dts.CHECKS = true;
            //            c = true;
            //        }
            //        catch
            //        {
            //            dts.CHECKS = false;
            //        }
            //        int tt = db.DOCUMENTOTS.Where(a => a.NUM_DOC.Equals(f.NUM_DOC) & a.TSFORM_ID == ts.POS).Count();
            //        if (tt == 0)
            //            ddt.Add(dts);
            //        else
            //            db.Entry(dts).State = EntityState.Modified;
            //    }
            //    if (ddt.Count > 0)
            //        db.DOCUMENTOTS.AddRange(ddt);
            //    db.SaveChanges();

            //    db.Dispose();
            //}

            FLUJO flujo = actual;
            flujo.ESTATUS = f.ESTATUS;
            flujo.FECHAM = DateTime.Now;
            flujo.COMENTARIO = f.COMENTARIO;
            flujo.USUARIOA_ID = User.Identity.Name;

            flujo.ID_RUTA_A = f.ID_RUTA_A;
            flujo.RUTA_VERSION = f.RUTA_VERSION;
            flujo.STEP_AUTO = f.STEP_AUTO;

            //Agregar funcionalidad, para checar si el próximo es contabilización, y si es contabilización 
            //checar que el usuario contabilizador esté asignado a la sociedad
            ContabilizarRes resc = new ContabilizarRes();
            if (d.ESTATUS == "F" && (d.ESTATUS_WF.Equals("P") | d.ESTATUS_WF.Equals("S")))
            {
                //MGC 30-10-2018 Modificación estatus, Pendiente por aprobadores  *@
                if (d.ESTATUS_PRE == "G")
                {
                    //Pendiente verificar quién es el dueño del flujo si C o A
                    if (User.Identity.Name == actual.USUARIOA_ID)
                    {

                        //Simular el pf.procesa
                        //ContabilizarRes res = new ContabilizarRes();
                        resc = pf.procesaConta(flujo);
                    }
                }
            }

            //Validar la respuesta
            //Hay respuesta
            if (resc.contabilizar != null && resc.res != null)
            {
                if (resc.contabilizar == true && resc.res == false)
                {
                    TempData["error"] = "Se necesita asignar usuario contabilizador a la sociedad: " + d.SOCIEDAD_ID;
                    return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        string res = pf.procesa(flujo, "", false, "", "");
                        if (res.Equals("0"))//Aprobado
                        {
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                        else if (res.Equals("1") | res.Equals("2") | res.Equals("3"))//CORREO
                        {
                            //return RedirectToAction("Enviar", "Mails", new { id = flujo.NUM_DOC, index = false, tipo = "A" });
                            //MGC 12092018
                            //Email em = new Email();
                            //string UrlDirectory = Request.Url.GetLeftPart(UriPartial.Path);
                            //string image = Server.MapPath("~/images/logo_kellogg.png");
                            //if (res.Equals("1") | res.Equals("2"))//CORREO
                            //{
                            //    em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Index", image);
                            //}
                            //else
                            //{
                            //    em.enviaMailC(f.NUM_DOC, true, Session["spras"].ToString(), UrlDirectory, "Details", image);
                            //}
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                        else
                        {
                            TempData["error"] = res;
                            return RedirectToAction("Details", "Solicitudes", new { id = flujo.NUM_DOC });
                        }
                    }
                }
            }

            int pagina = 103; //ID EN BASE DE DATOS
            using (WFARTHAEntities db = new WFARTHAEntities())
            {
                string u = User.Identity.Name;
                var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
                ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
                ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
                ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
                ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
                ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
                ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

                try
                {
                    string p = Session["pais"].ToString();
                    ViewBag.pais = p + ".png";
                }
                catch
                {
                    //ViewBag.pais = "mx.png";
                    //return RedirectToAction("Pais", "Home");
                }
                Session["spras"] = user.SPRAS_ID;
            }
            return View(f);
        }

        //public ActionResult Carga()
        //{
        //    int pagina = 601; //ID EN BASE DE DATOS
        //    using (TAT001Entities db = new TAT001Entities())
        //    {
        //        string u = User.Identity.Name;
        //        //string u = "admin";
        //        var user = db.USUARIOs.Where(a => a.ID.Equals(u)).FirstOrDefault();
        //        ViewBag.permisos = db.PAGINAVs.Where(a => a.ID.Equals(user.ID)).ToList();
        //        ViewBag.carpetas = db.CARPETAVs.Where(a => a.USUARIO_ID.Equals(user.ID)).ToList();
        //        ViewBag.usuario = user; ViewBag.returnUrl = Request.Url.PathAndQuery; ;
        //        ViewBag.rol = user.PUESTO.PUESTOTs.Where(a => a.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.Title = db.PAGINAs.Where(a => a.ID.Equals(pagina)).FirstOrDefault().PAGINATs.Where(b => b.SPRAS_ID.Equals(user.SPRAS_ID)).FirstOrDefault().TXT50;
        //        ViewBag.warnings = db.WARNINGVs.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();
        //        ViewBag.textos = db.TEXTOes.Where(a => (a.PAGINA_ID.Equals(pagina) || a.PAGINA_ID.Equals(0)) && a.SPRAS_ID.Equals(user.SPRAS_ID)).ToList();

        //        try
        //        {
        //            string p = Session["pais"].ToString();
        //            ViewBag.pais = p + ".png";
        //        }
        //        catch
        //        {
        //            //ViewBag.pais = "mx.png";
        //            //return RedirectToAction("Pais", "Home");
        //        }
        //        Session["spras"] = user.SPRAS_ID;
        //    }
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult Carga(IEnumerable<HttpPostedFileBase> files)
        //{
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult LoadExcel()
        //{
        //    List<DET_AGENTEC> ld = new List<DET_AGENTEC>();


        //    if (Request.Files.Count > 0)
        //    {
        //        HttpPostedFileBase file = Request.Files["FileUpload"];
        //        //using (var stream2 = System.IO.File.Open(url, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        //        //{
        //        string extension = System.IO.Path.GetExtension(file.FileName);
        //        // Auto-detect format, supports:
        //        //  - Binary Excel files (2.0-2003 format; *.xls)
        //        //  - OpenXml Excel files (2007 format; *.xlsx)
        //        //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream))
        //        //{
        //        IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
        //        // 2. Use the AsDataSet extension method
        //        DataSet result = reader.AsDataSet();

        //        // The result of each spreadsheet is in result.Tables
        //        // 3.DataSet - Create column names from first row
        //        DataTable dt = result.Tables[0];
        //        ld = objAList(dt);

        //        reader.Close();

        //    }

        //    List<Flujos> ff = new List<Flujos>();
        //    List<USUARIO> usuarios = new List<USUARIO>();
        //    List<CLIENTE> clientes = new List<CLIENTE>();
        //    List<PAI> paises = new List<PAI>();

        //    foreach (DET_AGENTEC da in ld)
        //    {
        //        Flujos f = new Flujos();
        //        ////---------------------------------------USUARIO
        //        f.USUARIOC_ID = da.USUARIOC_ID;
        //        f.USUARIOC_IDX = true;
        //        USUARIO u = usuarios.Where(x => x.ID.Equals(f.USUARIOC_ID)).FirstOrDefault();
        //        if (u == null)
        //        {
        //            u = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOC_ID) & x.ACTIVO == true).FirstOrDefault();
        //            if (u == null)
        //                f.USUARIOC_IDX = false;
        //            else
        //                usuarios.Add(u);
        //        }
        //        if (!f.USUARIOC_IDX)
        //            f.USUARIOC_ID = "<span class='red white-text'>" + f.USUARIOC_ID + "</span>";
        //        ////---------------------------------------PAIS
        //        f.PAIS_ID = da.PAIS_ID;
        //        f.PAIS_IDX = true;

        //        PAI p = paises.Where(x => x.LAND.Equals(f.PAIS_ID)).FirstOrDefault();
        //        if (p == null)
        //        {
        //            p = db.PAIS.Where(x => x.LAND.Equals(f.PAIS_ID) & x.ACTIVO == true & x.SOCIEDAD_ID != null).FirstOrDefault();
        //            if (p == null)
        //                f.PAIS_IDX = false;
        //            else
        //                paises.Add(p);
        //        }
        //        if (!f.PAIS_IDX)
        //            f.PAIS_ID = "<span class='red white-text'>" + f.PAIS_ID + "</span>";

        //        ////---------------------------------------CLIENTE
        //        f.KUNNR = da.KUNNR;
        //        f.KUNNRX = true;

        //        CLIENTE c = clientes.Where(x => x.KUNNR.Equals(f.KUNNR)).FirstOrDefault();
        //        if (c == null)
        //        {
        //            c = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(f.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //            if (c == null)
        //                f.KUNNRX = false;
        //            else
        //                clientes.Add(c);
        //        }
        //        if (!f.KUNNRX)
        //            f.KUNNR = "<span class='red white-text'>" + f.KUNNR + "</span>";


        //        f.VERSION = da.VERSION.ToString();
        //        f.POS = da.POS.ToString();
        //        ////---------------------------------------USUARIOA
        //        f.USUARIOA_ID = da.USUARIOA_ID;
        //        f.USUARIOA_IDX = true;
        //        USUARIO ua = usuarios.Where(x => x.ID.Equals(f.USUARIOA_ID)).FirstOrDefault();
        //        if (ua == null)
        //        {
        //            ua = db.USUARIOs.Where(x => x.ID.Equals(f.USUARIOA_ID) & x.ACTIVO == true).FirstOrDefault();
        //            if (ua == null)
        //                f.USUARIOA_IDX = false;
        //            else
        //                usuarios.Add(ua);
        //        }
        //        if (!f.USUARIOA_IDX)
        //            f.USUARIOA_ID = "<span class='red white-text'>" + f.USUARIOA_ID + "</span>";

        //        if (da.MONTO == null)
        //            f.MONTO = null;
        //        else
        //            f.MONTO = da.MONTO.ToString();
        //        f.PRESUPUESTO = da.PRESUPUESTO.ToString();
        //        ff.Add(f);
        //    }
        //    JsonResult jl = Json(ff, JsonRequestBehavior.AllowGet);
        //    return jl;
        //}

        //private string completa(string s, int longitud)
        //{
        //    string cadena = "";
        //    try
        //    {
        //        long a = Int64.Parse(s);
        //        int l = a.ToString().Length;

        //        //cadena = s;
        //        for (int i = l; i < longitud; i++)
        //        {
        //            cadena += "0";
        //        }
        //        cadena += a.ToString();
        //    }
        //    catch
        //    {
        //        cadena = s;
        //    }
        //    return cadena;
        //}

        //private List<DET_AGENTEC> objAList(DataTable dt)
        //{

        //    List<DET_AGENTEC> ld = new List<DET_AGENTEC>();
        //    List<CLIENTE> clientes = new List<CLIENTE>();
        //    //Rows
        //    var rowsc = dt.Rows.Count;
        //    //columns
        //    var columnsc = dt.Columns.Count;

        //    //Columnd and row to start
        //    var rows = 1; // 2
        //                  //var cols = 0; // A
        //    var pos = 1;

        //    for (int i = rows; i < rowsc; i++)
        //    {
        //        //for (var j = 0; j < columnsc; j++)
        //        //{
        //        //    var data = dt.Rows[i][j];
        //        //}
        //        if (i >= 4)
        //        {
        //            var v = dt.Rows[i][1];
        //            if (Convert.ToString(v) == "")
        //            {
        //                break;
        //            }
        //        }
        //        DET_AGENTEC doc = new DET_AGENTEC();

        //        string a = Convert.ToString(pos);

        //        doc.POS = Convert.ToInt32(a);
        //        try
        //        {
        //            doc.USUARIOC_ID = dt.Rows[i][0].ToString(); //Usuario creador

        //        }
        //        catch (Exception e)
        //        {
        //            doc.USUARIOC_ID = null;
        //        }
        //        try
        //        {
        //            doc.PAIS_ID = dt.Rows[i][1].ToString(); //País
        //        }
        //        catch (Exception e)
        //        {
        //            doc.PAIS_ID = null;
        //        }
        //        try
        //        {
        //            doc.KUNNR = dt.Rows[i][2].ToString();
        //            doc.KUNNR = completa(doc.KUNNR, 10);

        //            CLIENTE u = clientes.Where(x => x.KUNNR.Equals(doc.KUNNR)).FirstOrDefault();
        //            if (u == null)
        //            {
        //                u = db.CLIENTEs.Where(cc => cc.KUNNR.Equals(doc.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //                if (u == null)
        //                    doc.VKORG = null;
        //                else
        //                    clientes.Add(u);
        //            }

        //            CLIENTE c = clientes.Where(cc => cc.KUNNR.Equals(doc.KUNNR) & cc.ACTIVO == true).FirstOrDefault();
        //            if (c != null)
        //            {
        //                doc.VKORG = c.VKORG;
        //                doc.VTWEG = c.VTWEG;
        //                doc.SPART = c.SPART;
        //            }
        //            else
        //            {
        //                doc.VKORG = null;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            doc.KUNNR = null;
        //        }
        //        try
        //        {
        //            doc.POS = int.Parse(dt.Rows[i][3].ToString());
        //        }
        //        catch (Exception e)
        //        {
        //            doc.POS = 0;
        //        }
        //        try
        //        {
        //            doc.USUARIOA_ID = dt.Rows[i][4].ToString();
        //        }
        //        catch (Exception e)
        //        {
        //            doc.USUARIOA_ID = null;
        //        }

        //        try
        //        {
        //            doc.MONTO = decimal.Parse(dt.Rows[i][5].ToString());
        //        }
        //        catch (Exception e)
        //        {
        //            doc.MONTO = null;
        //        }
        //        try
        //        {
        //            string p = dt.Rows[i][6].ToString();
        //            if (p == "X" | p == "x")
        //                doc.PRESUPUESTO = true;
        //        }
        //        catch (Exception e)
        //        {
        //            doc.PRESUPUESTO = false;
        //        }

        //        //DET_AGENTEC poss = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) & x.PAIS_ID.Equals(doc.PAIS_ID)
        //        //& x.KUNNR.Equals(doc.KUNNR)).FirstOrDefault();
        //        //if (poss == null)
        //        //    pos = 1;
        //        //else
        //        //    pos = ld.Where(x => x.USUARIOC_ID.Equals(doc.USUARIOC_ID) & x.PAIS_ID.Equals(doc.PAIS_ID) & x.KUNNR.Equals(doc.KUNNR)).Count() + 1;

        //        ld.Add(doc);
        //        pos++;
        //    }
        //    return ld;
        //}
    }
}
