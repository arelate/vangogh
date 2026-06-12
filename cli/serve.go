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

	q := u.Query()

	portStr := q.Get(vangogh_integration.UrlPortParameter)
	port, err := strconv.Atoi(portStr)
	if err != nil {
		return err
	}

	operatingSystems := vangogh_integration.OperatingSystemsFromUrl(u)
	langCodes := vangogh_integration.LanguageCodesFromUrl(u)

	layout := vangogh_integration.DownloadsLayoutFromUrl(u)

	noPatches := q.Has(vangogh_integration.UrlNoPatchesParameter)

	rest.SetDefaultDownloadsFilters(operatingSystems, langCodes, noPatches)

	insecureCookies := q.Has(vangogh_integration.UrlInsecureCookiesParameter)
	stderr := q.Has(vangogh_integration.UrlStdErrParameter)

	return Serve(port, layout, insecureCookies, stderr)
}

// Serve starts a web server, listening to the specified port with optional logging
func Serve(port int, layout vangogh_integration.DownloadsLayout, insecureCookies bool, stderr bool) error {

	if stderr {
		nod.EnableStdErrLogger()
		nod.DisableOutput(nod.StdOut)
	}

	// migrate data before starting the web server
	if err := MigrateData(false); err != nil {
		return err
	}

	ia := nod.Begin("initializing server...")
	if err := rest.Init(layout, insecureCookies); err != nil {
		return err
	}
	ia.Done()

	rest.HandleFuncs()

	return http.ListenAndServe(fmt.Sprintf(":%d", port), nil)
}
