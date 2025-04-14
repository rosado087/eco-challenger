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
import { TopCompletedChallenges } from '../../../models/stats-models';

echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  BarChart,
  CanvasRenderer
])

@Component({
  selector: 'app-top-completed-challenges',
  imports: [NgxEchartsDirective],
  templateUrl: './top-completed-challenges.component.html',
  styleUrl: './top-completed-challenges.component.css',
  providers: [
    provideEchartsCore({ echarts }),
    PopupLoaderService
  ]
})
export class TopCompletedChallengesComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  option: EChartsCoreOption | null = null
  height = input<string>('300px')

  ngOnInit(): void {
    this.fetchChartData()
  }

  fetchChartData(): void {
    this.netApi
    .get<{challenges: TopCompletedChallenges[]}>('Statistics', 'top-completed-challenges')
    .subscribe({
        next: (r) => this.loadChartData(r.challenges),
        error: () => this.popupLoader.showPopup('Erro ao carregar top desafios completados.')
    })
  }

  loadChartData(data: TopCompletedChallenges[]): void {
    // Order from highest to lowest
    data.sort((a, b) => b.count - a.count)

    this.option = {
      title: {
        text: 'Top 10 Desafios Completados'
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
        type: 'category',
        data: data.map(c => c.challengeName),
        axisLabel: {
          rotate: 45,
          interval: 0
        }
      },
      yAxis: {
        type: 'value',
        minInterval: 1,
      },      
      series: [
        {
          name: 'Desafios',
          type: 'bar',
          data: data.map(c => c.count),
          itemStyle: {
            color: '#ed8355'
          },
        }
      ]
    }
  }
}
