import { Component, inject, input, OnInit } from '@angular/core';
import type { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { provideEchartsCore } from 'ngx-echarts'
import * as echarts from 'echarts/core'
import { BarChart } from 'echarts/charts'
import { CanvasRenderer } from 'echarts/renderers';
import { GridComponent, LegendComponent, TitleComponent, TooltipComponent } from 'echarts/components'
import { NetApiService } from '../../../services/net-api/net-api.service';
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service';
import { TopPurchasedTags } from '../../../models/stats-models';

echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  BarChart,
  CanvasRenderer
])

@Component({
  selector: 'app-top-purchased-tags',
  imports: [NgxEchartsDirective],
  templateUrl: './top-purchased-tags.component.html',
  styleUrl: './top-purchased-tags.component.css',
  providers: [
    provideEchartsCore({ echarts }),
    PopupLoaderService
  ]
})
export class TopPurchasedTagsComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  option: EChartsCoreOption | null = null
  height = input<string>('300px')

  ngOnInit(): void {
    this.fetchChartData()
  }

  fetchChartData(): void {
    this.netApi
    .get<{tags: TopPurchasedTags[]}>('Statistics', 'top-purchased-tags')
    .subscribe({
        next: (r) => this.loadChartData(r.tags),
        error: () => this.popupLoader.showPopup('Erro ao carregar top tags compradas.')
    })
  }

  loadChartData(data: TopPurchasedTags[]): void {
    // Order from highest to lowest
    data.sort((a, b) => b.count - a.count)

    this.option = {
      title: {
        text: 'Top Tags Compradas'
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow'
        }
      },
      legend: {},
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: {
        type: 'value',
        boundaryGap: [0, 0.01],
        minInterval: 1,
      },
      yAxis: {
        type: 'category',
        data: data.map(t => t.tagName)
      },
      series: [
        {
          name: 'Tag + ID',
          type: 'bar',
          data: data.map(t => t.count)
        }
      ]
    }
  }
}
