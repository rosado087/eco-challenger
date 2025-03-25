import { Component, input } from '@angular/core'
import { TableConfig } from '../../models/table'

@Component({
    selector: 'app-table',
    imports: [],
    templateUrl: './table.component.html',
    styleUrl: './table.component.css'
})
export class TableComponent {
    config = input<TableConfig>()
}
