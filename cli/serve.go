package cli

import (
	"fmt"
	"net/http"
	"net/url"
	"strconv"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest"
	"github.com/boggydigital/nod"
)

func ServeHandler(u *url.URL) error {
	portStr := vangogh_integration.ValueFromUrl(u, "port")
	port, err := strconv.Atoi(portStr)
	if err != nil {
		return err
	}

	oses := vangogh_integration.OperatingSystemsFromUrl(u)
	langCodes := vangogh_integration.LanguageCodesFromUrl(u)

	if len(oses) == 0 {
		oses = []vangogh_integration.OperatingSystem{vangogh_integration.AnyOperatingSystem}
	}

	noPatches := vangogh_integration.FlagFromUrl(u, "no-patches")

	layout := vangogh_integration.DownloadsLayoutFromUrl(u)

	rest.SetDefaultDownloadsFilters(oses, langCodes, noPatches)

	return Serve(port, layout, vangogh_integration.FlagFromUrl(u, "stderr"))
}

// Serve starts a web server, listening to the specified port with optional logging
func Serve(port int, layout vangogh_integration.DownloadsLayout, stderr bool) error {

	if stderr {
		nod.EnableStdErrLogger()
		nod.DisableOutput(nod.StdOut)
	}

	// migrate data before starting the web server
	if err := MigrateData(false); err != nil {
		return err
	}

	ia := nod.Begin("initializing server...")
	if err := rest.Init(layout); err != nil {
		return err
	}
	ia.Done()

	rest.HandleFuncs()

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
