// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package changes

import (
	"github.com/boggydigital/vangogh/internal/gog/local/schema"
	"time"
)

func (chg *schema.Change) Update(hash string) *schema.Change {
	if chg != nil {
		chg.Hash = hash
		chg.Modified = time.Now().Unix()
	}
	return chg
}
