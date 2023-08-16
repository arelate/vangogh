package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"net/http"
	"net/url"
)

func PostCompletionHandler(u *url.URL) error {
	cwu := vangogh_local_data.ValueFromUrl(u, "webhook-url")

	return PostCompletion(cwu)
}

func PostCompletion(webhookUrl string) error {

	pca := nod.Begin("posting completion...")
	defer pca.End()

	if webhookUrl == "" {
		pca.EndWithResult("empty webhook url")
		return nil
	}

	_, err := http.DefaultClient.Post(webhookUrl, "", nil)
	if err != nil {
		return pca.EndWithError(err)
	}

	pca.EndWithResult("done")

	return nil
}
