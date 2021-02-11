package cmd

import (
	"encoding/json"
	"fmt"
	"github.com/boggydigital/kvas"
	"log"
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
	case Products:
		return true
	default:
		return false
	}
}

func Download(ids []string, pt, media, kind string) error {
	switch kind {
	case "image":
		return downloadImages(ids, pt, media)
	default:
		fmt.Println("download", pt, media, kind, ids)
	}
	return nil
}

func downloadImages(ids []string, pt, media string) error {
	if !hasImages(pt) {
		return fmt.Errorf("type %s (%s) doesn't contain images", pt, media)
	}

	if len(ids) == 0 {
		log.Printf("no ids specified to download")
		return nil
	}

	dstUrl, err := destinationUrl(pt, media)
	if err != nil {
		return err
	}

	kvProductImages, err := kvas.NewLocal(dstUrl, ".json")
	if err != nil {
		return err
	}

	for _, id := range ids {
		imgRc, err := kvProductImages.Get(id)
		if err != nil {
			return err
		}

		var ii productImage
		if err := json.NewDecoder(imgRc).Decode(&ii); err != nil {
			return err
		}

		fmt.Println(ii.Image)

		if err := imgRc.Close(); err != nil {
			return err
		}
	}

	return nil
}
