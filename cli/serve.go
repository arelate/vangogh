package cli

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
	"strconv"
)

func ServeHandler(u *url.URL) error {
	portStr := vangogh_integration.ValueFromUrl(u, "port")
	port, err := strconv.Atoi(portStr)
	if err != nil {
		return err
	}

	oses := vangogh_integration.OperatingSystemsFromUrl(u)
	langCode := vangogh_integration.ValuesFromUrl(u, vangogh_integration.LanguageCodeProperty)

	if len(oses) == 0 {
		oses = []vangogh_integration.OperatingSystem{vangogh_integration.AnyOperatingSystem}
	}
	if len(langCode) == 0 {
		langCode = []string{"en"}
	}

	noPatches := vangogh_integration.FlagFromUrl(u, "no-patches")

	rest.SetDefaultDownloadsFilters(oses, langCode, noPatches)

	sharedUsername := vangogh_integration.ValueFromUrl(u, "shared-username")
	sharedPassword := vangogh_integration.ValueFromUrl(u, "shared-password")
	adminUsername := vangogh_integration.ValueFromUrl(u, "admin-username")
	adminPassword := vangogh_integration.ValueFromUrl(u, "admin-password")

	rest.SetUsername(rest.SharedRole, sharedUsername)
	rest.SetPassword(rest.SharedRole, sharedPassword)
	rest.SetUsername(rest.AdminRole, adminUsername)
	rest.SetPassword(rest.AdminRole, adminPassword)

	return Serve(port, vangogh_integration.FlagFromUrl(u, "stderr"))
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
