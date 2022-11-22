import React, { useEffect, useState, useRef } from "react";
import moment from "moment/moment";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import { Line, getElementAtEvent } from "react-chartjs-2";

const Chart = (props) => {
  ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
  );

  const { arrData, type } = props;
  const [datasets, setDatasets] = useState([]);
  const [title, setTitle] = useState("");
  const [labels, setLabels] = useState([]);
  const chartRef = useRef();

  const onClick = (event) => {
    // console.log(getElementAtEvent(chartRef.current, event));
  };

  useEffect(() => {
    if (arrData && Object.keys(arrData).length > 0 && props && type) {
      const arr = [];
      if (type === "int") {
        arrData.data.map((val) => {
          arr.push({
            label: val.name,
            data: formatDate(val.dataInt, arrData.labels),
            borderColor: val.color,
            backgroundColor: val.color,
          });
        });
      }

      if (type === "double") {
        arrData.data.map((val) => {
          arr.push({
            label: val.name,
            data: formatDate(val.dataDouble, arrData.labels),
            borderColor: val.color,
            backgroundColor: val.color,
          });
        });
      }

      setDatasets(arr);
      setTitle(arrData.title);
      setLabels(arrData.labels);
    } else {
      setDatasets([]);
      setTitle("");
      setLabels([]);
    }
  }, [arrData, props, type]);

  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: "top",
      },
      title: {
        display: true,
        text: title,
      },
    },
  };

  const formatDate = (data, labels) => {
    const arrCount = [];
    labels.map((val) => {
      data.map((val1) => {
        if (val === val1.date) {
          if (type === "int") {
            arrCount.push(val1.count);
          }
          if (type === "double") {
            arrCount.push(val1.value);
          }
        }
      });
    });
    return arrCount;
  };

  const formatDay = (data) => {
    var arr = [];
    data.map((val) => {
      arr.push(moment(new Date(val).toISOString()).format("DD-MM"));
    });

    return arr;
  };

  const data = {
    labels: formatDay(labels),
    datasets: datasets,
  };

  return (
    <Line options={options} data={data} ref={chartRef} onClick={onClick} />
  );
};

export default Chart;
