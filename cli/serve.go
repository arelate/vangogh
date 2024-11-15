package cli

import (
	"fmt"
	"github.com/arelate/vangogh/rest"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"strconv"
)

func ServeHandler(u *url.URL) error {
	portStr := vangogh_local_data.ValueFromUrl(u, "port")
	port, err := strconv.Atoi(portStr)
	if err != nil {
		return err
	}

	oses := vangogh_local_data.OperatingSystemsFromUrl(u)
	langCode := vangogh_local_data.ValuesFromUrl(u, vangogh_local_data.LanguageCodeProperty)

	if len(oses) == 0 {
		oses = []vangogh_local_data.OperatingSystem{vangogh_local_data.AnyOperatingSystem}
	}
	if len(langCode) == 0 {
		langCode = []string{"en"}
	}

	noPatches := vangogh_local_data.FlagFromUrl(u, "no-patches")

	rest.SetDefaultDownloadsFilters(oses, langCode, noPatches)

	sharedUsername := vangogh_local_data.ValueFromUrl(u, "shared-username")
	sharedPassword := vangogh_local_data.ValueFromUrl(u, "shared-password")
	adminUsername := vangogh_local_data.ValueFromUrl(u, "admin-username")
	adminPassword := vangogh_local_data.ValueFromUrl(u, "admin-password")

	rest.SetUsername(rest.SharedRole, sharedUsername)
	rest.SetPassword(rest.SharedRole, sharedPassword)
	rest.SetUsername(rest.AdminRole, adminUsername)
	rest.SetPassword(rest.AdminRole, adminPassword)

	return Serve(port, vangogh_local_data.FlagFromUrl(u, "stderr"))
}

// Serve starts a web server, listening to the specified port with optional logging
func Serve(port int, stderr bool) error {

	if stderr {
		nod.EnableStdErrLogger()
		nod.DisableOutput(nod.StdOut)
	}

	if err := rest.Init(); err != nil {
		return err
	}

	rest.HandleFuncs()

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
