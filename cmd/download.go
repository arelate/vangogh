package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_urls"
	"github.com/boggydigital/kvas"
	"log"
	"net/url"
)

type productImage struct {
	Image string `json:"image"`
}

func hasImages(pt string) bool {
	switch pt {
	case WishlistProducts:
		fallthrough
	case AccountProducts:
		fallthrough
	case StoreProducts:
		return true
	default:
		return false
	}
}

func Download(ids []string, pt, media, kind string, all bool) error {
	switch kind {
	case "image":
		return downloadImages(ids, pt, media, all)
	default:
		fmt.Println("download", pt, media, kind, ids)
	}
	return nil
}

func downloadImages(ids []string, pt, media string, all bool) error {
	if !hasImages(pt) {
		return fmt.Errorf("type %s (%s) doesn't contain images", pt, media)
	}

	dstUrl, err := destinationUrl(pt, media)
	if err != nil {
		return err
	}

	kvProductImages, err := kvas.NewJsonLocal(dstUrl)
	if err != nil {
		return err
	}

	if all {
		ids = kvProductImages.All()
	}

	if len(ids) == 0 {
		log.Printf("no ids specified to download")
		return nil
	}

	//httpClient, err := internal.HttpClient()
	//if err != nil {
	//	return err
	//}

	//dc := dolo.NewClient(httpClient, nil)

	for _, id := range ids {

		fmt.Printf("downloading image for %s (%s) id %s\n", pt, media, id)

		imgRc, err := kvProductImages.Get(id)
		if err != nil {
			return err
		}

		var ii productImage
		if err := json.NewDecoder(imgRc).Decode(&ii); err != nil {
			return err
		}

		// this should be gog_urls func
		u, err := url.Parse(ii.Image)
		if err != nil {
			return err
		}
		u.Scheme = gog_urls.HttpsScheme
		u.Path += ".png"

		fmt.Println(u.String())

		//if err := dc.Download(u, "images", false); err != nil {
		//	return err
		//}

		if err := imgRc.Close(); err != nil {
			return err
		}
	}

	return nil
}
