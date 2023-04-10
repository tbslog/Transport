import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import LoadingPage from "../Common/Loading/LoadingPage";

const UpdateDocFile = (props) => {
  const { dataClick, refeshData, hideModal } = props;

  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    setValue,
    watch,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    TenChungTu: {
      required: "Không được để trống",
    },
    LoaiChungTu: {
      required: "Không được để trống",
    },
    TrangThai: {
      required: "Không được để trống",
    },
  };

  const [listStatus, setListStatus] = useState([]);
  const [listDocType, setListDocType] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      const listDocType = await getData("Common/GetListDocType");
      if (listDocType && listDocType.length > 0) {
        let arr = [];
        listDocType.forEach((element) => {
          arr.push({
            label: element.tenLoaiChungTu,
            value: element.maLoaiChungTu,
          });

          setListDocType(arr);
        });
      }
      SetIsLoading(false);
    })();
  }, []);

  useEffect(() => {
    if (listDocType && listDocType.length > 0) {
      if (dataClick && Object.keys(dataClick).length > 0) {
        (async () => {
          let getDoc = await getData(
            `BillOfLading/GetDocById?docId=${dataClick.maChungTu}`
          );
          if (getDoc && Object.keys(getDoc).length > 0) {
            setValue("TenChungTu", getDoc.tenChungTu);
            setValue(
              "LoaiChungTu",
              listDocType.find((x) => x.value === getDoc.loaiChungTu)
            );
            setValue("GhiChu", getDoc.ghiChu);
            setValue("TrangThai", getDoc.trangThai);
            setValue("fileDoc", []);
            setValue("MaDieuPhoi", getDoc.maDieuPhoi);
          }
        })();
      }
    }
  }, [dataClick, listDocType]);

  const onSubmit = async (data) => {
    SetIsLoading(true);

    let create = await postData(
      `BillOfLading/UpdateDoc?docId=${dataClick.maChungTu}`,
      {
        MaDieuPhoi: data.MaDieuPhoi,
        fileImage: !data.fileDoc ? null : data.fileDoc[0],
        TenChungTu: data.TenChungTu,
        LoaiChungTu: data.LoaiChungTu.value,
        GhiChu: data.GhiChu,
      },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );

    if (create === 1) {
      refeshData(data.MaDieuPhoi);
      hideModal();
    }
    SetIsLoading(false);
  };

  return (
    <>
      <div className="card card-primary">
        <div>
          {IsLoading === true && (
            <div>
              <LoadingPage></LoadingPage>
            </div>
          )}
        </div>

        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TenChungTu">Tên Chứng Từ(*)</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="TenChungTu"
                      {...register("TenChungTu", Validate.TenChungTu)}
                    />
                    {errors.TenChungTu && (
                      <span className="text-danger">
                        {errors.TenChungTu.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiChungTu">Loại Chứng Từ(*)</label>
                    <Controller
                      name="LoaiChungTu"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          classNamePrefix={"form-control"}
                          value={field.value}
                          options={listDocType}
                        />
                      )}
                      rules={{ required: "không được để trống" }}
                    />
                    {errors.LoaiChungTu && (
                      <span className="text-danger">
                        {errors.LoaiChungTu.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="GhiChu">Ghi Chú</label>
                    <input
                      type="text"
                      className="form-control"
                      id="GhiChu"
                      placeholder="Nhập ghi chú"
                      {...register("GhiChu")}
                    />
                    {errors.GhiChu && (
                      <span className="text-danger">
                        {errors.GhiChu.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="TrangThai">Trạng Thái(*)</label>
                    <select
                      className="form-control"
                      {...register("TrangThai", Validate.TrangThai)}
                    >
                      <option value="">Chọn Trạng Thái</option>
                      {listStatus &&
                        listStatus.map((val) => {
                          return (
                            <option value={val.statusId} key={val.statusId}>
                              {val.statusContent}
                            </option>
                          );
                        })}
                    </select>
                    {errors.TrangThai && (
                      <span className="text-danger">
                        {errors.TrangThai.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="fileDoc">Tải lên tệp Chứng Từ(*)</label>
                    <input
                      type="file"
                      className="form-control-file"
                      {...register("fileDoc", Validate.fileDoc)}
                    />
                    {errors.fileDoc && (
                      <span className="text-danger">
                        {errors.fileDoc.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
            </div>
            <div className="card-footer">
              <div>
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ float: "right" }}
                >
                  Cập Nhật
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default UpdateDocFile;
