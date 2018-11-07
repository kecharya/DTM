using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DTM_Preview.Models;
using epi = VUMC.EnterpriseServices;

namespace DTM_Preview.Controllers
{
    public class PatientInfoController : Controller
    {
        // GET: PatientInfo
        public ActionResult Index(string mrNumber)
        {
            if (!string.IsNullOrWhiteSpace(mrNumber))
            {
                //return RedirectToAction("Patient", new { mrn = mrNumber });    
                return RedirectToAction("PatientModel", new { mrn = mrNumber });
            }
            else
            {
                return View();
            }
        }

        public ActionResult Patient(string mrn)
        {
            RetrievePatientInformation info = new RetrievePatientInformation();
            List<Patient> patients = new List<Patient>();
            patients = info.GetPatientInformation(mrn);
            return View(patients);
        }

        public ActionResult PatientModel(string mrn)
        {
            RetrievePatientInformation info = new RetrievePatientInformation();
            Patient pat = new Models.Patient();
            pat = info.GetPatient(mrn);
            return View(pat);
        }
        // GET: PatientInfo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PatientInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientInfo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientInfo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientInfo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientInfo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientInfo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
