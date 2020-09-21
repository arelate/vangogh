// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package remote

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
)

type AccountProductPage struct {
	*Page
}

func NewAccountProductPage(client *http.Client, mt media.Type) *AccountProductPage {
	return &AccountProductPage{
		&Page{
			Source: NewSource(client, urls.AccountProductsPageURL(mt, false, false)),
		},
	}
}
