// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package urls

import (
	media "github.com/boggydigital/vangogh/internal/gog/media"
	"net/url"
	"strconv"
)

const dateAdded = "date_added"

func WishlistPageURL(mt media.Type, hidden bool) *url.URL {
	wishlistPage := &url.URL{
		Scheme: HttpsScheme,
		Host:   GogHost,
		Path:   WishlistPath,
	}
	hiddenFlag := "0"
	if hidden {
		hiddenFlag = "1"
	}
	q := wishlistPage.Query()
	q.Add("mediaType", strconv.Itoa(int(mt)))
	q.Add("sortBy", dateAdded)
	q.Add("hiddenFlag", hiddenFlag)
	wishlistPage.RawQuery = q.Encode()

	return wishlistPage

}
