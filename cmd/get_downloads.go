package cmd

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"strings"
)

const (
	windows = "windows"
	macos   = "macos"
	linux   = "linux"
)

type DownloadManifest struct {
	gog_types.ManualDownload
	Title           string
	Language        string
	OperatingSystem string
	DLC             bool
	Extras          bool
}

func createManifests(title, lang, os string, dlc, extras bool, manualDownloads []gog_types.ManualDownload) []DownloadManifest {
	dms := make([]DownloadManifest, 0, len(manualDownloads))
	for _, md := range manualDownloads {
		dms = append(dms,
			DownloadManifest{
				ManualDownload:  md,
				Title:           title,
				Language:        lang,
				OperatingSystem: os,
				DLC:             dlc,
				Extras:          extras,
			})
	}
	return dms
}

func getDetailsDownloads(det *gog_types.Details, dlc bool, langNames, osSet gost.StrSet, extras bool) ([]DownloadManifest, error) {

	manifests := make([]DownloadManifest, 0)

	if det == nil {
		return manifests, fmt.Errorf("details are nil")
	}

	downloads, err := det.GetDownloads()
	if err != nil {
		return manifests, err
	}

	for _, dl := range downloads {
		if !langNames.Has(dl.Language) {
			continue
		}
		if osSet.Has(windows) && len(dl.Windows) > 0 {
			manifests = append(
				manifests,
				createManifests(det.Title, dl.Language, windows, dlc, false, dl.Windows)...)
		}
		if osSet.Has(macos) && len(dl.Mac) > 0 {
			manifests = append(
				manifests,
				createManifests(det.Title, dl.Language, macos, dlc, false, dl.Mac)...)
		}
		if osSet.Has(linux) && len(dl.Linux) > 0 {
			manifests = append(
				manifests,
				createManifests(det.Title, dl.Language, linux, dlc, false, dl.Linux)...)
		}

		for _, dlcDetails := range det.DLCs {
			dlcDownloads, err := getDetailsDownloads(&dlcDetails, true, langNames, osSet, extras)
			if err != nil {
				return manifests, err
			}
			manifests = append(manifests, dlcDownloads...)
		}
	}

	if extras {
		manifests = append(
			manifests,
			createManifests(det.Title, "", "", dlc, true, det.Extras)...)
	}

	return manifests, nil
}

func GetDownloads(ids []string, slug string, os []string, langCodes []string, extras bool, all bool) error {

	idSet := gost.StrSetWith(ids...)

	vrDetails, err := vangogh_values.NewReader(vangogh_products.Details, gog_media.Game)
	if err != nil {
		return err
	}

	exl, err := vangogh_extracts.NewList(
		vangogh_properties.TitleProperty,
		vangogh_properties.NativeLanguageNameProperty,
		vangogh_properties.SlugProperty)
	if err != nil {
		return err
	}

	langNames := gost.NewStrSet()
	for _, lc := range langCodes {
		langName, ok := exl.Get(vangogh_properties.NativeLanguageNameProperty, lc)
		if !ok || langName == "" {
			continue
		}
		langNames.Add(langName)
	}

	if slug != "" {
		slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
		idSet.Add(slugIds...)
	}

	osSet := gost.StrSetWith(os...)
	downloadManifests := make(map[string][]DownloadManifest, 0)

	for _, id := range idSet.All() {

		det, err := vrDetails.Details(id)
		if err != nil {
			return err
		}

		detDownloads, err := getDetailsDownloads(det, false, langNames, osSet, extras)
		if err != nil {
			return err
		}

		downloadManifests[id] = detDownloads
	}

	//httpClient, err := internal.HttpClient()
	//if err != nil {
	//	return err
	//}

	//TODO: Write detailed plan for downloading files

	for id, dms := range downloadManifests {
		title, _ := exl.Get(vangogh_properties.TitleProperty, id)
		fmt.Println(id, title)
		for _, dm := range dms {
			msg := ""
			if dm.Extras {
				msg = fmt.Sprintf(" Extras: %s: %s\n", dm.Name, dm.ManualUrl)
			} else {
				name := dm.Name
				if dm.DLC {
					name = strings.TrimPrefix(dm.Title, title)
					name = strings.TrimPrefix(name, ": ")
					name = strings.TrimPrefix(name, " - ")
					name = "DLC: " + name
				}
				msg = fmt.Sprintf(" %s %s (%s, %s): %s\n",
					name,
					dm.Version,
					dm.Language,
					dm.OperatingSystem,
					dm.ManualUrl)
			}
			fmt.Printf(msg)
		}

		//resp, err := httpClient.Head(gog_urls.ManualDownload(dm.ManualUrl).String())
		//if err != nil {
		//	return err
		//}
		//
		//redirectedUrl := resp.Request.URL
		//redirectedUrl.Path += ".xml"
		//
		//fmt.Println(" validation:", redirectedUrl)
		//
		//if err := resp.Body.Close(); err != nil {
		//	return err
		//}
		//
		//break
	}
	return nil

}
