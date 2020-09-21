// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package remote

import (
	"github.com/boggydigital/vangogh/internal/gog/local"
)

type Transferor interface {
	Transfer(id int, setter local.Setter) error
}
