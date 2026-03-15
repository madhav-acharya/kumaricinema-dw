using KumariCinema.Models;
using KumariCinema.Services;
using System;
using System.Collections.Generic;
using System.Web;

namespace KumariCinema.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CurrentUser"] == null)
            {
                HttpContext.Current.Response.Redirect("~/components/Login.aspx");
                return;
            }

            AppUser user = (AppUser)Session["CurrentUser"];
            userNameLabel.Text = user.Name;

            if (!IsPostBack)
            {
                ApplySidebarPermissions(user);
            }
        }

        private void ApplySidebarPermissions(AppUser user)
        {
            var auth = new AuthorizationService();
            var lockedLinks = new List<string>();

            if (!auth.CanManageUsers(user)) lockedLinks.Add("usersLink");
            if (!auth.CanManageTheaters(user)) lockedLinks.Add("theatersLink");
            if (!auth.CanManageMovies(user, user.TheaterId))
            {
                lockedLinks.Add("moviesLink");
                lockedLinks.Add("genresLink");
                lockedLinks.Add("languagesLink");
            }
            if (!auth.CanManageShows(user, user.TheaterId))
            {
                lockedLinks.Add("showsLink");
                lockedLinks.Add("hallsLink");
                lockedLinks.Add("seatsLink");
                lockedLinks.Add("seatTypesLink");
                lockedLinks.Add("ticketsLink");
            }
            if (!auth.CanViewBookings(user, user.TheaterId)) lockedLinks.Add("bookingsLink");
            if (!auth.CanManagePayments(user, user.TheaterId)) lockedLinks.Add("paymentsLink");

            if (lockedLinks.Count == 0)
            {
                return;
            }

            string ids = string.Join(",", lockedLinks);
            string script = @"(function(){
                var ids='" + ids + @"'.split(',');
                ids.forEach(function(id){
                    var link=document.getElementById(id);
                    if(!link) return;
                    if(link.getAttribute('data-locked')==='1') return;
                    link.setAttribute('data-locked','1');
                    link.style.opacity='0.65';
                    link.insertAdjacentHTML('beforeend',' <i class=\""fas fa-lock\""></i>');
                    link.addEventListener('click', function(e){
                        e.preventDefault();
                        if(typeof showToast === 'function'){
                            showToast('You do not have permission to access this page', 'error');
                        }
                        setTimeout(function(){
                            window.location.href = 'unauthorized.aspx';
                        }, 350);
                    });
                });
            })();";

            Page.ClientScript.RegisterStartupScript(GetType(), "locks", script, true);
        }
    }
}
