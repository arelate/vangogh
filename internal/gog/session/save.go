// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package session

import (
	"github.com/boggydigital/vangogh/internal/storage"
	"net/http"
)

const cookiesFilename = "cookies.json"

func Save(cookies []*http.Cookie) error {
	return storage.Save(cookies, cookiesFilename)
}
