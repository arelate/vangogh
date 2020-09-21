// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package changes

import (
	"github.com/boggydigital/vangogh/internal/gog/local/schema"
	"time"
)

func New(id int, hash string) *schema.Change {
	return &schema.Change{
		ID:       id,
		Hash:     hash,
		Added:    time.Now().Unix(),
		Modified: time.Now().Unix(),
	}
}
