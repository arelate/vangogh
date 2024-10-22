package rest

import (
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/kevlar"
	"net/http"
	"strings"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func GetDownloads(w http.ResponseWriter, r *http.Request) {

	// GET /downloads?id

	if err := RefreshRedux(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	id := r.URL.Query().Get("id")

	dls, err := getDownloads(id, operatingSystems, languageCodes, excludePatches, rdx)
	if err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		return
	}

	p := compton_pages.Downloads(id, dls, rdx)
	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}

func getDownloads(id string,
	operatingSystems []vangogh_local_data.OperatingSystem,
	languageCodes []string,
	excludePatches bool,
	rdx kevlar.ReadableRedux) (vangogh_local_data.DownloadsList, error) {

	vrDetails, err := vangogh_local_data.NewProductReader(vangogh_local_data.Details)
	if err != nil {
		return nil, err
	}

	det, err := vrDetails.Details(id)
	if err != nil {
		return nil, err
	}

	dl := make(vangogh_local_data.DownloadsList, 0)

	if det != nil {
		dl, err = vangogh_local_data.FromDetails(det, rdx)
		if err != nil {
			return nil, err
		}
	}

	return dl.Only(operatingSystems,
		[]vangogh_local_data.DownloadType{vangogh_local_data.AnyDownloadType},
		languageCodes,
		excludePatches), nil
}

func getClientOperatingSystem(r *http.Request) vangogh_local_data.OperatingSystem {

	var clientOS vangogh_local_data.OperatingSystem

	//attempt to extract platform from user agent client hints first
	secChUaPlatform := r.Header.Get("Sec-CH-UA-Platform")

	switch secChUaPlatform {
	case "Linux":
		clientOS = vangogh_local_data.Linux
	case "iOS":
		fallthrough
	case "macOS":
		clientOS = vangogh_local_data.MacOS
	case "Windows":
		clientOS = vangogh_local_data.Windows
	default:
		// "Android", "Chrome OS", "Chromium OS" or "Unknown"
		clientOS = vangogh_local_data.AnyOperatingSystem
	}

	if clientOS != vangogh_local_data.AnyOperatingSystem {
		return clientOS
	}

	//use "User-Agent" header if we couldn't extract platform from user agent client hints
	userAgent := r.UserAgent()

	if strings.Contains(userAgent, "Windows") {
		clientOS = vangogh_local_data.Windows
	} else if strings.Contains(userAgent, "Mac OS X") {
		clientOS = vangogh_local_data.MacOS
	} else if strings.Contains(userAgent, "Linux") {
		clientOS = vangogh_local_data.Linux
	}

	return clientOS
}
